using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
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
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var content = new MimeMessage();
            content.From.Add(new MailboxAddress(options.NotifyAccount));
            content.To.Add(new MailboxAddress(email));
            content.Subject = subject;
            content.Body = new TextPart() { Text = message };
            using (var client = new SmtpClient())
            {   
                client.ServerCertificateValidationCallback = (s, c, h, e) => options.NotifySsl;
                client.Connect(options.NotifyServer, options.NotifyPort, false);
                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(options.NotifyAccount, options.NotifyPassword);
                client.Send(content);
                client.Disconnect(true);
            }
            return Task.FromResult(0);
        }
    }
}
