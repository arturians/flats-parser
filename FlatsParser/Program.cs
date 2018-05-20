﻿using System;
using System.Linq;

namespace FlatsParser
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ProgramConfigurator().GetConfiguration(args);

            var flats = new FlatsInfoProvider().GetFlats();
            var flatsLocalStorageProvider = new FlatsLocalStorageProvider(configuration.FlatsLocalStoragePath);
            var previousSavedFlats = flatsLocalStorageProvider.GetLatest();
            flatsLocalStorageProvider.Store(flats);

            var distincts = new FlatsComparator(previousSavedFlats, flats).GetDistincts();
            if (distincts.Any())
            {
                new EmailNotifier(distincts, configuration).Notify();
                Console.WriteLine("Notified to email");
                new GoogleSheetExporter().Export(flats);
                Console.WriteLine("Stored to google sheets");
            }
            else
            {
                Console.WriteLine("No changes!");
            }

        }
    }
}
