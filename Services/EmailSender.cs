using IdentityBasicDotNetCore.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace IdentityBasicDotNetCore.Services
{
    public class EmailSender : IEmailSender
    {
        private IOptions<SmtpOptions> _smtpOptions;

        public EmailSender(IOptions<SmtpOptions> smtpOptions)
        {
            _smtpOptions = smtpOptions;       
        }
        public async Task SendEmailAsync(string from, string to, string subject, string message)
        {
            var mailmessage = new MailMessage(from, to, subject, message);

            using (var client = new SmtpClient(_smtpOptions.Value.Host, _smtpOptions.Value.Port)
            {
                Credentials = new NetworkCredential(_smtpOptions.Value.Username, _smtpOptions.Value.Password),
              //  UseDefaultCredentials = true,
                EnableSsl = true,
              //  DeliveryMethod = SmtpDeliveryMethod.Network,
            })
            {
                await client.SendMailAsync(mailmessage);
            }

        }
    }
}
