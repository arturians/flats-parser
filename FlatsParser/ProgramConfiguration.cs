namespace FlatsParser
{
    public class ProgramConfiguration
    {
        public string FlatsLocalStoragePath { get; set; }
        public string EmailAuthor { get; set; }
        public string EmailPassword { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public bool EnableSsl { get; set; }
        public string EmailRecipients { get; set; }
    }
}