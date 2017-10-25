using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace FlatsParser
{
    internal class Program
    {
        private const int FirstId = 1449;
        private const int LastId = 1739;
        private const int FlatsCount = LastId - FirstId;

        public static void Main(string[] args)
        {
            
            var flatIds = Enumerable.Range(FirstId, FlatsCount);

            var flats = flatIds
                .AsParallel().Select(id =>
            {
                using (var client = new WebClient {Encoding = Encoding.UTF8})
                {
                    var url = $"http://www.xn--80adrpkbapik.xn--p1ai/flats/{id}";
                    var downloadString = client.DownloadString(url);
                    var flat = RegexPageParser.ParsePage(downloadString);
                    flat.Id = id;
                    flat.Url = url;
                    Console.WriteLine($"Added {id}");
                    return flat;
                }
            }).ToList();

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
