using System;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Config;

namespace FlatsParser
{
	internal static class Program
	{
		public static void Main(string[] args)
		{
			XmlConfigurator.Configure(new System.IO.FileInfo("log4netConfig.xml"));
			var logger = LogManager.GetLogger(Assembly.GetEntryAssembly().FullName);

			var iterationId = Guid.NewGuid();
			logger.Info($"{Environment.NewLine}Start new iteration {iterationId}");
			try
			{
				var configuration = new ProgramConfigurator().GetConfiguration(args);
				var flats = new IslandsFlatsProvider(1400, 3000)
					.GetFlats()
					.Where(flat => flat.BuildName.Equals(configuration.AllowedBuildName, StringComparison.InvariantCulture))
					.ToArray();
				var flatsLocalStorageProvider = new FlatsLocalStorageProvider(configuration.FlatsLocalStoragePath);
				var previousSavedFlats = flatsLocalStorageProvider.GetLatest();
				flatsLocalStorageProvider.Store(flats);
				var distincts = new FlatsComparator(previousSavedFlats, flats).GetDistincts();
				if (distincts.Any())
				{
					new EmailNotifier(distincts, configuration).Notify();
					logger.Info($"Notified to email {configuration.EmailRecipients} about {distincts.Count}");

					if (!string.IsNullOrEmpty(configuration.GoogleSpreadsheetId))
					{
						new GoogleSheetExporter(configuration.GoogleSpreadsheetId).Export(flats);
						logger.Info("Stored to google sheets");
					}
					else
					{
						logger.Warn("Can't send info to Google spreadsheet");
					}
				}
				else
				{
					logger.Info("No changes!");
				}
			}
			catch (Exception e)
			{
				logger.Error("Some error occured", e);
			}
			logger.Info($"Finish iteration {iterationId}");
		}
	}
}
