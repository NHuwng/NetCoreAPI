using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MailKit.Security;
using DemoMvc.Migrations;

namespace DemoMVC.Models.Process
{
    public class SendMailService : IEmailSender
    {
        private readonly MailSettings mailSettings;
        private readonly ILogger<SendMailService> logger;
        public SendMailService (IOptions<MailSettings> _mailSetting, ILogger<SendMailService>_logger)
        {
            mailSettings = _mailSetting.Value;
            logger = _logger;
            logger.LogInformation("Create sendMailService");

        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.Sender = new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail);
            message.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;
            var builder = new BodyBuilder();
            builder.HtmlBody = htmlMessage;
            message.Body = builder.ToMessageBody();
            using var smtb = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                smtb.Connect(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
                smtb.Authenticate(mailSettings.Mail, mailSettings.Password);
                await smtb.SendAsync(message);
            }
            catch(Exception ex)
            {
                // gửi mail thất bại, nọi dung email sẽ được lưu vào thư mục mailsSave
                System.IO.Directory.CreateDirectory("MailsSave");
                var emailSaveFile = string.Format(@"mailsSave/{0}.eml",Guid.NewGuid);
                await message.WriteToAsync(emailSaveFile);

                logger.LogInformation("Lỗi gửi mail, lưu lại - "+emailSaveFile);
                logger.LogError(ex.Message);

            }
            smtb.Disconnect(true);
            logger.LogInformation("Send mail to:" + email);
        }
    }
}