namespace BenaaInvitations.API.Models
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; } = string.Empty;
        public string SmtpPass { get; set; } = string.Empty;
        public string FromName { get; set; } = "Benaa Platform | بوابة بناء";
        public string FromEmail { get; set; } = "it@hns.edu.sa";
    }
}
