using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

namespace FlatsParser
{
	internal class GoogleSheetExporter : ResultExporter
	{
		private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
		private const string ApplicationName = "Flats Parser";
		private const string SpreadsheetId = "1J1JAErjEF9Z6-qodsUHz4MST84EoVkF3BgaAotR6SHk";

		public override void Export(IEnumerable<Flat> flats)
		{
			var service = CreateService();
			var latestSheetProperties = GetLatestSheetProperties(service);
			var newSheetInfo = GetNewSheetInfo();
			var duplicateSheetRequest = GetDuplicateRequest(newSheetInfo, latestSheetProperties);
			var updateCellsRequests = GetUpdateRequests(newSheetInfo, latestSheetProperties, flats);

			var batchUpdateSpreadsheetRequest = new BatchUpdateSpreadsheetRequest
			{
				Requests = new List<Request>()
			};
			batchUpdateSpreadsheetRequest.Requests.Add(new Request {DuplicateSheet = duplicateSheetRequest});
			foreach (var request in updateCellsRequests)
			{
				batchUpdateSpreadsheetRequest.Requests.Add(new Request { UpdateCells = request });
			}
			var batchUpdateRequest = service.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, SpreadsheetId);
			batchUpdateRequest.Execute();
		}

		private static SheetsService CreateService()
		{
			UserCredential credential;

			using (var stream =
				new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
			{
				var credPath = Environment.GetFolderPath(
					Environment.SpecialFolder.Personal);
				credPath = Path.Combine(credPath, ".credentials/sheets.googleapis.com-dotnet-quickstart.json");

				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.Load(stream).Secrets,
					Scopes,
					"user",
					CancellationToken.None,
					new FileDataStore(credPath, true)).Result;
				Console.WriteLine("Credential file saved to: " + credPath);
			}

			return new SheetsService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = ApplicationName,
			});
		}

		private static SheetProperties GetLatestSheetProperties(SheetsService service)
		{
			var getRequest = service.Spreadsheets.Get(SpreadsheetId);
			var spreadsheet = getRequest.Execute();
			return spreadsheet.Sheets.FirstOrDefault()?.Properties;
		}

		private static NewSheetInfo GetNewSheetInfo()
		{
			return new NewSheetInfo
			{
				Id = (int)(DateTime.Now - new DateTime(2017, 1, 1)).TotalSeconds,
				Name = DateTime.Now.ToString("yyyy-MM-dd_HH:mm")
			};
		}

		private static DuplicateSheetRequest GetDuplicateRequest(NewSheetInfo newSheetInfo, SheetProperties latestSheetProperties)
		{
			return new DuplicateSheetRequest
			{
				SourceSheetId = latestSheetProperties.SheetId,
				InsertSheetIndex = 0,
				NewSheetId = newSheetInfo.Id,
				NewSheetName = newSheetInfo.Name
			};
		}

		private static List<UpdateCellsRequest> GetUpdateRequests(NewSheetInfo newSheetInfo, SheetProperties latestSheetProperties, IEnumerable<Flat> flats)
		{
			var updateCellsRequests = new List<UpdateCellsRequest> { GetUpdateFlatsRequest(newSheetInfo, flats)};
			updateCellsRequests.AddRange(GetUpdateStatsRequest(newSheetInfo, latestSheetProperties));
			return updateCellsRequests;
		}

		private static UpdateCellsRequest GetUpdateFlatsRequest(NewSheetInfo newSheetInfo, IEnumerable<Flat> flats)
		{

			var rows = flats.Select(f => new RowData
			{
				Values = new List<CellData>
				{
					new CellData {UserEnteredValue = new ExtendedValue {NumberValue = f.Number}},
					new CellData {UserEnteredValue = new ExtendedValue {StringValue = $"{f.CurrentState}"}},
					new CellData {UserEnteredValue = new ExtendedValue {NumberValue = f.Section}},
					new CellData {UserEnteredValue = new ExtendedValue {NumberValue = f.Floor}},
					new CellData {UserEnteredValue = new ExtendedValue {NumberValue = f.RoomsCount}},
					new CellData {UserEnteredValue = new ExtendedValue {NumberValue = f.LivingArea}},
					new CellData {UserEnteredValue = new ExtendedValue {NumberValue = f.TotalArea}},
					new CellData {UserEnteredValue = new ExtendedValue {NumberValue = f.Price}},
					new CellData {UserEnteredValue = new ExtendedValue {NumberValue = f.AreaRatio}},
					new CellData {UserEnteredValue = new ExtendedValue {NumberValue = f.PricePerMetre}},
					new CellData {UserEnteredValue = new ExtendedValue {NumberValue = f.KitchenArea}},
					new CellData {UserEnteredValue = new ExtendedValue {NumberValue = f.Id}},
					new CellData {UserEnteredValue = new ExtendedValue {StringValue = f.Url}}
				}
			}).ToList();

			return new UpdateCellsRequest
			{
				Start = new GridCoordinate { SheetId = newSheetInfo.Id, ColumnIndex = 0, RowIndex = 1 }, //cell A2
				Fields = "userEnteredValue",
				Rows = rows
			};
		}

		private static IEnumerable<UpdateCellsRequest> GetUpdateStatsRequest(NewSheetInfo newSheetInfo, SheetProperties latestSheetProperties)
		{
			return new[] {2, 4, 6}
				.Select(rowIndex => new UpdateCellsRequest
				{
					Start = new GridCoordinate {SheetId = newSheetInfo.Id, ColumnIndex = 14, RowIndex = rowIndex - 1},
					Fields = "userEnteredValue",
					Rows = new List<RowData>
					{
						new RowData
						{
							Values = new List<CellData>
							{
								new CellData
								{
									UserEnteredValue =
										new ExtendedValue {FormulaValue = $"=N{rowIndex}-'{latestSheetProperties.Title}'!N{rowIndex}"}
								}
							}
						}
					}
				})
				.ToList();
		}
	}

	internal class NewSheetInfo
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
}
