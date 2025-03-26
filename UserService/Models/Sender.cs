namespace UserService.Models
{
    public class Sender
    {
        public string SmtpAddress { get; set; }
        public int PortNumber { get; set; }
        public bool EnableSSL { get; set; }
        public string EmailFrom { get; set; }
        public string Password { get; set; }
    }
}
