namespace UserService.Services.Interface
{
    public interface IEmailSenderService
    {
        void SendEmail(string toEmail, string subject, string body);
    }
}
