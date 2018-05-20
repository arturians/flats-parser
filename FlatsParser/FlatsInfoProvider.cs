using System;
using System.Linq;
using System.Net;
using System.Text;

namespace FlatsParser
{
    public class FlatsInfoProvider
    {
        private const int FirstFlatId = 1449;
        private const int LastFlatId = 1739;
        private const int FlatsCount = LastFlatId - FirstFlatId;

        public Flat[] GetFlats()
        {
            var flatIds = Enumerable.Range(FirstFlatId, FlatsCount);
            var flats = flatIds.AsParallel().Select(id =>
            {
                using (var client = new WebClient { Encoding = Encoding.UTF8 })
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
            flats.Sort((flat, flat1) => flat.Number - flat1.Number); //note: ascending sort

            return flats.ToArray();
        }
    }
}