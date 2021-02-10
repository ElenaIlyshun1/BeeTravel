using BeeTravel.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
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
                var emailMessage = new MimeMessage();

                emailMessage.From.Add(new MailboxAddress("Администрация сайта", "beetravelconfirm@gmail.com"));
                emailMessage.To.Add(new MailboxAddress("lenailyshun@gmail.com", to));
                emailMessage.Subject = "hello";
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = text
                };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587);
                    await client.AuthenticateAsync("beetravelconfirm@gmail.com", "Qw3eI98*63%");              
                    await client.SendAsync(emailMessage);

                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                
            }
            
        }
    }
}
