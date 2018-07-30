using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace FlatsParser
{
    public class EmailNotifier
    {
        private readonly List<Tuple<Flat, Flat>> _distincts;
        private readonly ProgramConfiguration _programConfiguration;

        public EmailNotifier(List<Tuple<Flat, Flat>> distincts, ProgramConfiguration programConfiguration)
        {
            _distincts = distincts;
            _programConfiguration = programConfiguration;
        }

        public void Notify()
        {
            var body = CreateBody();
            using (var smtpClient = new SmtpClient(_programConfiguration.SmtpHost, _programConfiguration.SmtpPort)
            {
                Credentials = new NetworkCredential(_programConfiguration.EmailAuthor, _programConfiguration.EmailPassword),
                EnableSsl = _programConfiguration.EnableSsl
            })
            {
                try
                {
                    smtpClient.Send(_programConfiguration.EmailAuthor, _programConfiguration.EmailRecipients, $"Изменения от {DateTime.UtcNow:u}", body);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private string CreateBody()
        {
            var stringBuilder = new StringBuilder();
            foreach (var distinct in _distincts)
            {
                var old = distinct.Item1;
                var latest = distinct.Item2;
                stringBuilder.Append($"{Environment.NewLine}{old.RoomsCount}к. квартира {old.Number:000}({old.Floor:00} этаж, {old.Section} секция, url: {latest.Url} )");
                if (old.Price != latest.Price)
                {
                    var priceDistinct = (latest.Price - old.Price).ToString("+0;-#");
                    stringBuilder.Append($"{Environment.NewLine}Цена: {old.Price} -> {latest.Price} = {priceDistinct}");
                }
                if (old.CurrentState != latest.CurrentState)
                    stringBuilder.Append($"{Environment.NewLine}Статус: {old.CurrentState} -> {latest.CurrentState}");
                stringBuilder.Append(Environment.NewLine);
            }
            return stringBuilder.ToString();
        }
    }
}