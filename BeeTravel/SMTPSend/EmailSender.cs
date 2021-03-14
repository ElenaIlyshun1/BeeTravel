using BeeTravel.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeTravel.SMTPSend
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string to, string subject, string text, bool isHtml = true)
        {
            try
            {
                var apiKey = "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";
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress("BeeTravel@i.ua", "no-reply");
                var toEmail = new EmailAddress(to, "Client");
                var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, text, text);
                var response = await client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                
            }
            
        }
    }
}
