using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace ZenlessZoneZeroWiki.Services
{
    public class EmailService
    {
        private readonly string smtpHost = "smtp.gmail.com";
        private readonly int smtpPort = 587;
        private readonly string smtpUser = "aloysiuspacheco2003@gmail.com";
        private readonly string smtpPass = "axim wcdg nolf oovg";
        private readonly string fromEmail = "aloysiuspacheco2003@gmail.com";

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var smtpClient = new SmtpClient(smtpHost)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
} 