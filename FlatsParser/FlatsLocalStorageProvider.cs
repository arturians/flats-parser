using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace FlatsParser
{
    public class FlatsLocalStorageProvider
    {
        private readonly string _flatsLocalStoragePath;

        public FlatsLocalStorageProvider(string flatsLocalStoragePath)
        {
            _flatsLocalStoragePath = flatsLocalStoragePath;
        }

        public Flat[] GetLatest()
        {
            var directoryInfo = new DirectoryInfo(_flatsLocalStoragePath);
            var name = directoryInfo
                .GetFiles()
                .OrderByDescending(f => f.LastWriteTime)
                .FirstOrDefault()?
                .FullName;
            if (string.IsNullOrEmpty(name))
                throw new FileNotFoundException("File with previous saved flats not found!");
            var jsonFlats = File.ReadAllText(name);
            var deserializeObject = JsonConvert.DeserializeObject<Flat[]>(jsonFlats);
            return deserializeObject;
        }

        public void Store(Flat[] flats)
        {
            var serializeObject = JsonConvert.SerializeObject(flats, Formatting.Indented);
            var currentFileName = $"{DateTime.UtcNow:yyyy_MM_dd_HH_mm}_utc.txt";
            var fullName = Path.Combine(_flatsLocalStoragePath, currentFileName);
            File.WriteAllText(fullName, serializeObject);
        }
    }
}