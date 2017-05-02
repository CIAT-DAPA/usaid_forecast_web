using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools;
using MailKit.Net.Smtp;
using MailKit.Security;
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
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            /*MailMessage mail = new MailMessage()
            {
                From = new MailAddress(_emailSettings.UsernameEmail, "Muhammad Hassan Tariq")
            };*/
            var content = new MimeMessage();
            content.From.Add(new MailboxAddress(options.NotifyAccount));
            content.To.Add(new MailboxAddress(email));
            content.Subject = subject;
            content.Body = new TextPart() { Text = message };            
            using (var client = new SmtpClient())
            {
                //client.ServerCertificateValidationCallback = (s, c, h, e) => options.NotifySsl;
                //await client.ConnectAsync(options.NotifyServer, options.NotifyPort, SecureSocketOptions.StartTlsWhenAvailable).ConfigureAwait(false);
                await client.ConnectAsync(options.NotifyServer, options.NotifyPort, options.NotifySsl).ConfigureAwait(false);
                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(options.NotifyAccount, options.NotifyPassword);
                client.Send(content);
                client.Disconnect(true);
            }
        }
    }
}
