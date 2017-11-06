using System;
using System.Linq;
using System.Net;
using System.Text;

namespace FlatsParser
{
    internal static class Program
    {
        private const int FirstId = 1449;
        private const int LastId = 1739;
        private const int FlatsCount = LastId - FirstId;

        public static void Main(string[] args)
        {
            
            var flatIds = Enumerable.Range(FirstId, FlatsCount);

            var flats = flatIds.AsParallel().Select(id =>
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
            Console.WriteLine($"Done{Environment.NewLine}Exporting to file...");
            new FileExporter().Export(flats);
	        Console.WriteLine($"Done{Environment.NewLine}Exporting to Google Sheets");
			new GoogleSheetExporter().Export(flats);
	        Console.WriteLine("Done");
        }
    }
}
