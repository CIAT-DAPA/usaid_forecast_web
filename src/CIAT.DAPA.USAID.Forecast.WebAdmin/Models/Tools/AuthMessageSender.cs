using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools
{
    public class AuthMessageSender : IEmailSender
    {
        /// <summary>
        /// Get or set the global options
        /// </summary>
        private Settings options { get; set; }

        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="options"></param>
        public AuthMessageSender(IOptions<Settings> options)
        {
            this.options = options.Value;
        }

        /// <summary>
        /// Method that search a email
        /// </summary>
        /// <param name="email">Destination email</param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var content = new MimeMessage();
            content.From.Add(new MailboxAddress(options.NotifyAccount));
            content.To.Add(new MailboxAddress(email));
            content.Subject = subject;
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = message;
            content.Body = bodyBuilder.ToMessageBody();           
            using (var client = new SmtpClient())
            {
                client.Connect(options.NotifyServer, options.NotifyPort, options.NotifySsl);
                client.Authenticate(options.NotifyAccount, options.NotifyPassword);
                client.Send(content);
                client.Disconnect(true);
            }
        }
    }
}
