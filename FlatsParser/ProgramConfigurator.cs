using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace FlatsParser
{
    public class ProgramConfigurator
    {
        public ProgramConfiguration GetConfiguration(string[] arguments)
        {
            if (arguments == null || !arguments.Any())
                return GetDefaultConfiguration();

            for (var i = 0; i < arguments.Length; i++)
            {
                if (!arguments[i].Equals("-c"))
                    continue;
                ThrowIfArgumentsSizeIncorrect(arguments, i);
                var pathToConfiguration = arguments[i + 1];
                var text = File.ReadAllText(pathToConfiguration);
                var configuration = JsonConvert.DeserializeObject<ProgramConfiguration>(text);
                return configuration;
            }

            return GetDefaultConfiguration();
        }

        private static ProgramConfiguration GetDefaultConfiguration()
        {
            var localFilesStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "LocalStorage");
            return new ProgramConfiguration { FlatsLocalStoragePath = localFilesStoragePath };
        }

        private static void ThrowIfArgumentsSizeIncorrect(IReadOnlyList<string> arguments, int currentPosition)
        {
            if (currentPosition + 1 >= arguments.Count)
                throw new ArgumentException($"Argument {arguments[currentPosition]} hasn't value!");
        }
    }
}