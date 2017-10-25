using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace FlatsParser
{
    internal class FileExporter : IResultExporter
    {
        public void Export(IEnumerable<Flat> flats)
        {
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
                            $"{(f.KitchenArea.HasValue ? FormatDouble(f.KitchenArea.Value) : "-")}\t" +
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
