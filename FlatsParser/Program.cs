using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace FlatsParser
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var client = new WebClient { Encoding = Encoding.UTF8 };
            var flats = new List<Flat>();
            for (var id = 1449; id < 1739; id++)
            {
                var url = $"http://www.xn--80adrpkbapik.xn--p1ai/flats/{id}";
                var downloadString = client.DownloadString(url);
                var flat = RegexPageParser.ParsePage(downloadString);
                flat.Id = id;
                flat.Url = url;

                flats.Add(flat);
                Console.WriteLine($"Added {id}");
            }
            Console.WriteLine("Sorting...");
            flats.Sort((flat, flat1) => flat1.Number - flat.Number);
            Console.WriteLine("Sorted");
            using (var fs = File.Create($"{DateTime.Now:yyyy_MM_dd}.txt"))
            {
                foreach (var f in flats)
                {
                    var s = $"{f.Number}\t" +
                        $"{f.CurrentState}\t" +
                        $"{f.Section}\t" +
                        $"{f.Floor}\t" +
                        $"{f.RoomsCount}\t" +
                        $"{FormatDouble(f.LivingArea)}\t" +
                        $"{FormatDouble(f.TotalArea)}\t" +
                        $"{f.Price}\t" +
                        $"{FormatDouble(f.AreaRatio)}\t" +
                        $"{FormatDouble(f.PricePerMetre)}\t" +
                        $"{(f.KitchenArea.HasValue ? FormatDouble(f.KitchenArea.Value) :  "-")}\t" +
                        $"{f.Id}\t" +
                        $"{f.Url}{Environment.NewLine}";
                    var bytes = Encoding.UTF8.GetBytes(s);
                    fs.Write(bytes, 0, bytes.Length);
                }
            }   
        }

        private static string FormatDouble(double d)
        {
            return d.ToString("F", CultureInfo.GetCultureInfo("ru-RU"));
        }
    }
}
