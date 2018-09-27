using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using log4net;

namespace FlatsParser
{
	public class EmailNotifier
	{
		private readonly List<FlatsDistinct> distincts;
		private readonly ProgramConfiguration programConfiguration;
		private readonly ILog logger;

		public EmailNotifier(List<FlatsDistinct> distincts, ProgramConfiguration programConfiguration)
		{
			this.distincts = distincts;
			this.programConfiguration = programConfiguration;
			logger = LogManager.GetLogger(GetType());
		}

		public bool Notify()
		{
			var body = CreateBody();
			var buildNames = GetBuildNames();

			using (var smtpClient = new SmtpClient(programConfiguration.SmtpHost, programConfiguration.SmtpPort)
			{
				Credentials = new NetworkCredential(programConfiguration.EmailAuthor, programConfiguration.EmailPassword),
				EnableSsl = programConfiguration.EnableSsl
			})
			{
				try
				{
					smtpClient.Send(programConfiguration.EmailAuthor, programConfiguration.EmailRecipients, $"Изменения {buildNames} от {DateTime.UtcNow:u}", body);
					return true;
				}
				catch (Exception e)
				{
					logger.Error("Some error ocurred when send distincts", e);
				}
			}

			return false;
		}

		private string GetBuildNames()
		{
			var hashSet = new HashSet<string>();
			foreach (var flatsDistinct in distincts)
			{
				if (flatsDistinct.LatestState != null)
					hashSet.Add(flatsDistinct.LatestState.BuildName);
				if (flatsDistinct.PreviousState != null)
					hashSet.Add(flatsDistinct.PreviousState.BuildName);
			}

			return string.Join(",", hashSet);
		}

		private string CreateBody()
		{
			var stringBuilder = new StringBuilder();
			foreach (var distinct in distincts)
			{
				var old = distinct.PreviousState;
				var latest = distinct.LatestState;

				var flat = latest ?? old;

				if (flat == null)
				{
					logger.Error("Found NULL flat, skip it to send");
					continue;
				}

				stringBuilder.AppendLine($"{flat.RoomsCount}к. квартира {flat.Number:000}({flat.Floor:00} этаж, {flat.Section} секция, url: {flat.Url} )");
				if (old == null)
				{
					stringBuilder.AppendLine($"Новая информация о квартире с id {latest.Id}");
				}
				else if (latest == null)
				{
					stringBuilder.AppendLine($"Исчезла информация о квартире с предыдущим Id {old.Id}");
				}
				else
				{
					if (old.Price != latest.Price)
					{
						var priceDistinct = (latest.Price - old.Price).ToString("+0;-#");
						stringBuilder.AppendLine($"Цена: {old.Price} -> {latest.Price} = {priceDistinct}");
					}
					if (old.CurrentState != latest.CurrentState)
						stringBuilder.AppendLine($"Статус: {old.CurrentState} -> {latest.CurrentState}");
				}
				stringBuilder.AppendLine(Environment.NewLine);
			}
			return stringBuilder.ToString();
		}
	}
}