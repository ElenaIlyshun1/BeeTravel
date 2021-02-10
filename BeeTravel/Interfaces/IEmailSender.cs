using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeTravel.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string text, bool isHtml = true);
    }
}
