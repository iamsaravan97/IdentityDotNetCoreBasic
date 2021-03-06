using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityBasicDotNetCore.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string from, string to, string subject, string message);
    }
}
