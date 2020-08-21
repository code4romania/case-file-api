using CaseFile.Api.Core.Options;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CaseFile.Api.Core.Services
{
    public interface IEmailService
    {
        Task Send(string to, string subject, string body);
    }
    public class EmailService : IEmailService
    {
        private readonly EmailOptions _emailOptions;

        public EmailService(IOptions<EmailOptions> emailOptions)
        {
            _emailOptions = emailOptions.Value;
        }

        public async Task Send(string to, string subject, string body)
        {            
            var client = new SendGridClient(_emailOptions.SendGridApiKey);
            var message = new SendGridMessage();
            message.From = new EmailAddress(_emailOptions.SupportEmail, _emailOptions.SupportName);
            message.AddTo(new EmailAddress(to));
            message.Subject = subject;
            message.HtmlContent = body;
            message.PlainTextContent = body;

            await client.SendEmailAsync(message);
        }
    }
}
