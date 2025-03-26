using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;
using UserService.Models;
using UserService.Services.Interface;

namespace UserService.Services.Implement
{
    public class EmailSenderService : Interface.IEmailSenderService
    {
        private readonly Sender _config;

        public EmailSenderService(Sender config)
        {
            _config = config;
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(_config.EmailFrom);
                    mail.To.Add(toEmail);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = false;

                    using (SmtpClient smtp = new SmtpClient(_config.SmtpAddress, _config.PortNumber))
                    {
                        smtp.Credentials = new NetworkCredential(_config.EmailFrom, _config.Password);
                        smtp.EnableSsl = _config.EnableSSL;
                        smtp.Send(mail);
                        Console.WriteLine("Email đã được gửi thành công!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi gửi email: " + ex.Message);
            }
        }
    }
}
