using System;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FlatsParser
{
    internal class RegexPageParser
    {
        public static Flat ParsePage(string downloadString)
        {
            var flat = new Flat();
            ParseMainInfo(downloadString, flat);
            ParseAreaSection(downloadString, flat);
            ParsePrice(downloadString, flat);
            ParseState(downloadString, flat);
            return flat;
        }

        private static void ParsePrice(string downloadString, Flat flat)
        {
            var priceRegex = new Regex(@"\<span class\=\'b\-book-apartment__price b-price-style1'\>\n(?'price'([0-9]* )*)");
            var priceMatch = priceRegex.Match(downloadString);
            if (priceMatch.Success)
            {
                var value = priceMatch.Groups["price"].Value;
                var price = value.Replace(" ", string.Empty);
                flat.Price = int.Parse(price);
            }
        }

        private static void ParseMainInfo(string downloadString, Flat flat)
        {
            var mainInfoRegex =
                new Regex(
                    @"Новый город, (?'Quarter'\d) квартал — Секция № (?'Section'\d{1}) — (?'Floor'\d{1,2}) этаж — Квартира № (?'Number'\d{1,3}) — Острова",
                    RegexOptions.IgnoreCase);
            var match = mainInfoRegex.Match(downloadString);
            if (!match.Success)
                return;
            flat.Section = int.Parse(match.Groups["Section"].Value);
            flat.Floor = int.Parse(match.Groups["Floor"].Value);
            flat.Number = int.Parse(match.Groups["Number"].Value);
        }

        private static void ParseAreaSection(string downloadString, Flat flat)
        {
	        var actions = new Action<double>[]
	        {
		        d => flat.RoomsCount = (int) d,
		        d => flat.TotalArea = d,
		        d => flat.LivingArea = d,
		        d =>
		        {
			        if ((int) d != flat.Floor) throw new DataException("Floor mismatch");
		        },
		        d => flat.KitchenArea = d
	        };

            var regex = new Regex(@"\<span class=\'level\'\>(?'number'([0-9]*(\.|\,)[0-9]*)|([0-9]*))(?!кв.)",
                RegexOptions.Multiline);
            var numberMatch = regex.Matches(downloadString);
            for (var i = 0; i < numberMatch.Count; i++)
            {
                var match1 = numberMatch[i];
                if (!match1.Success)
                    continue;
                var str = match1.Groups["number"].Value;
                if (string.IsNullOrEmpty(str))
                    continue;
                str = str.Replace(',', '.');
	            double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var d);
	            actions[i].Invoke(d);
            }
        }

        private static void ParseState(string downloadString, Flat flat)
        {
            if (downloadString.Contains("Квартира продана"))
            {
                flat.CurrentState = State.Sold;
                return;
            }
            if (downloadString.Contains("Забронировано"))
            {
                flat.CurrentState = State.Reserved;
                return;
            }
            flat.CurrentState = State.Free;
        }
    }
}