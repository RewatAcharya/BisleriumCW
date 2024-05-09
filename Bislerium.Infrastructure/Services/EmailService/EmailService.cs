using Bislerium.Application.IServices;
using Bislerium.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Infrastructure.Services.EmailService
{
    public class EmailService : IEmailService
    {
        public async Task SendMail(EmailMessage email)
        {
            string fromMail = "zackzig55@gmail.com";
            string password = "cocr zhew jitc ceyk";

            using MailMessage message = new();
            message.From = new MailAddress(fromMail);
            message.Subject = email.Subject;
            message.To.Add(new MailAddress(email.To));
            message.Body = email.Body;
            message.IsBodyHtml = true;
            using SmtpClient smtpClient = new("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential(fromMail, password);
            smtpClient.EnableSsl = true;
            smtpClient.Send(message);
        }
    }
}
