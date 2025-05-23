﻿
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using iFredApps.TimeTracker.Core.Interfaces.Services;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace iFredApps.TimeTracker.Core.Services
{

   public class EmailService : IEmailService
   {
      private readonly EmailSettings _emailSettings;

      public EmailService(IOptions<EmailSettings> emailSettings)
      {
         _emailSettings = emailSettings.Value;
      }

      public async Task SendEmailAsync(string toEmail, string subject, string body)
      {
         var email = new MimeMessage();
         email.From.Add(MailboxAddress.Parse(_emailSettings.SenderEmail));
         email.To.Add(MailboxAddress.Parse(toEmail));
         email.Subject = subject;
         email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

         using var smtp = new SmtpClient();
         await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
         await smtp.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
         var result = await smtp.SendAsync(email);
         await smtp.DisconnectAsync(true);
      }
   }

}
