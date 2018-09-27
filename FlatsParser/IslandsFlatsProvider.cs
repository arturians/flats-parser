using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using log4net;

namespace FlatsParser
{
	public class IslandsFlatsProvider
	{
		private const string urlTemplate = "http://xn--b1acdssgdar0i.xn--80adrpkbapik.xn--p1ai/flats/flat.php?ELEMENT_ID=";
		private readonly int minId;
		private readonly int maxId;
		private readonly ILog logger;

		public IslandsFlatsProvider(int minId, int maxId)
		{
			this.minId = minId;
			this.maxId = maxId;
			logger = LogManager.GetLogger(GetType());
		}

		public Flat[] GetFlats()
		{
			var flats = Enumerable
				.Range(minId, maxId - minId + 1)
				.AsParallel()
				.Select(ParseSave)
				.Where(flat => flat != null)
				.OrderBy(flat => flat.Number)
				.ToArray();
			return flats;
		}

		private Flat ParseSave(int id)
		{
			logger.Info($"Processing {id}");
			try
			{
				var document = new HtmlWeb().Load($"{urlTemplate}{id}");
				return ParseInternal(document, id);
			}
			catch (Exception e)
			{
				logger.Warn(e.Message);
				return null;
			}
		}

		private Flat ParseInternal(HtmlDocument document, int id)
		{
			var parameters = new Dictionary<string, string>();
			LoadTableFields(document, "//div[@class='pl_block_3_tab_right']//ul//li", parameters);
			LoadTableFields(document, "//div[@class='pl_block_3_tab_left']//ul//li", parameters);

			var flat = new Flat
			{
				Id = id,
				Url = urlTemplate + id,
				Number = int.Parse(parameters["Номер квартиры"]),
				Floor = int.Parse(parameters["Этаж"]),
				TotalArea = ParseBeginOfString(parameters["Общая площадь"]),
				LivingArea = ParseBeginOfString(parameters["Жилая площадь"]),
				FlatHight = ParseBeginOfString(parameters["Высота потолков"]),
				Decoration = parameters["Отделка"].Equals("Чистовая") ? Decoration.Full : Decoration.None,
				Deadline = parameters["Срок сдачи"],
				IsClassicKitchen = parameters["Тип кухни"].Equals("Классическая"),
				Bathroom = (int)ParseBeginOfString(parameters["Санузлы"]),
				Balcony = (int)ParseBeginOfString(parameters["Лоджия"])
			};

			SetlBuildAndSection(document, flat);
			SetRoomsCount(document, flat);
			SetPrice(document, flat);
			SetState(document, flat);

			//var floorPlanAttributes = document.DocumentNode.SelectSingleNode("//img[@class='plan-pdf_right-img']").Attributes;
			//var floorPlanUrl = floorPlanAttributes.AttributesWithName("src").FirstOrDefault()?.Value ?? throw new DataException("Floor plan URL not found.");
			logger.Debug($"{flat.Id} {flat.BuildName} {flat.Number}");
			return flat;
		}

		private static void SetState(HtmlDocument document, Flat flat)
		{
			var divText = document.DocumentNode.SelectSingleNode("//div[@class='pl_block_3_title']").InnerText;
			flat.CurrentState = State.Free;
			if (divText.Contains("Продана"))
			{
				flat.CurrentState = State.Sold;
			}
			else if (divText.Contains("Забронирована"))
			{
				flat.CurrentState = State.Reserved;
			}
		}

		private static void LoadTableFields(HtmlDocument document, string xPath, IDictionary<string, string> parameters)
		{
			var table = document.DocumentNode.SelectNodes(xPath);
			if (table == null || !table.Any())
				throw new DataException($"Data not found by xPath '{xPath}'.");
			foreach (var node in table)
			{
				parameters.Add(node.FirstChild.InnerText, node.LastChild.InnerText);
			}
		}

		private static void SetlBuildAndSection(HtmlDocument document, Flat flat)
		{
			var divText = document.DocumentNode.SelectSingleNode("//div[@class='pl_tabs_box_right_sec_con']").InnerText;
			var match = new Regex(@"(?'build'(\s|\S)+), СЕКЦИЯ (?'section'\d{1,2})").Match(divText);
			if (!match.Success)
				throw new DataException("Can not find build and section information.");
			flat.Section = int.Parse(match.Groups["section"].Value);
			flat.BuildName = match.Groups["build"].Value.Trim();
		}

		private static void SetRoomsCount(HtmlDocument document, Flat flat)
		{
			var divText = document.DocumentNode.SelectSingleNode("//span[@class='kv_title']").InnerText.Trim();
			if (divText.StartsWith("Студия", StringComparison.InvariantCultureIgnoreCase))
			{
				flat.RoomsCount = 0;
				return;
			}

			var match = new Regex(@"(?'roomsCount'\d)-").Match(divText);
			if (!match.Success)
				throw new DataException("Can not find rooms count information.");
			flat.RoomsCount = int.Parse(match.Groups["roomsCount"].Value);
		}

		private void SetPrice(HtmlDocument document, Flat flat)
		{
			var innerText = document.DocumentNode.SelectNodes("//div[@class='pl_block_3_price_con_price']").First().InnerText;
			flat.Price = ParseBeginOfString(innerText.Trim().Replace(" ", ""));
		}

		private decimal ParseBeginOfString(string input)
		{
			var chrResult = input.TakeWhile(c => char.IsDigit(c) || c == '.').ToArray();
			return decimal.Parse(new string(chrResult), CultureInfo.InvariantCulture);
		}
	}
}