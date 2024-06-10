using MailKit.Net.Smtp;
using MailKit.Security;
using map_tile_server.Models.Configurations;
using map_tile_server.Models.Details;
using Microsoft.Extensions.Options;
using MimeKit;
using Serilog;

namespace map_tile_server.Services

{
    public class EmailService : IEmailService
    {
        private readonly IEmailSettings _mailSettings;

        public EmailService(IEmailSettings settings)
        {
            _mailSettings = settings;
        }
        public async Task SendEmailForgetPassword(MailRequest mailRequest)
        {
            Log.Information("{@a}", mailRequest);
     
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Email);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Email, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);

        }
    }
}
