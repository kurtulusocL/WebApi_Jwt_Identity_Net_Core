using Identity_Jwt_WebApi.Business.Abstract;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Identity_Jwt_WebApi.Business.Concrete
{
    public class MailManager : IMailService
    {
        private readonly IConfiguration _configuration;
        public MailManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string content)
        {
            var apiKey = _configuration["MailApiKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("kurtulusocal@gmail.com", "Simple Title");
            var to = new EmailAddress("kurtulusocal@outlook.com", "Simple Title");
            var htmlContent = "<strong>content text</strong>" + DateTime.Now.ToLocalTime();
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, htmlContent);
            var responsive = await client.SendEmailAsync(msg);
        }
    }
}
