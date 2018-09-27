using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace FlatsParser
{
    public class FlatsLocalStorageProvider
    {
        private readonly string flatsLocalStoragePath;

        public FlatsLocalStorageProvider(string flatsLocalStoragePath)
        {
            this.flatsLocalStoragePath = flatsLocalStoragePath;
        }

        public Flat[] GetLatest()
        {
            var directoryInfo = new DirectoryInfo(flatsLocalStoragePath);
            var name = directoryInfo
                .GetFiles()
                .OrderByDescending(f => f.LastWriteTime)
                .FirstOrDefault()?
                .FullName;
            if (string.IsNullOrEmpty(name))
                return new Flat[0];
            var jsonFlats = File.ReadAllText(name);
            var deserializeObject = JsonConvert.DeserializeObject<Flat[]>(jsonFlats);
            return deserializeObject;
        }

        public void Store(Flat[] flats)
        {
            var serializeObject = JsonConvert.SerializeObject(flats, Formatting.Indented, new JsonSerializerSettings());
            var currentFileName = $"{DateTime.UtcNow:yyyy_MM_dd_HH_mm}_utc.txt";
            var fullName = Path.Combine(flatsLocalStoragePath, currentFileName);
            File.WriteAllText(fullName, serializeObject);
        }
    }
}