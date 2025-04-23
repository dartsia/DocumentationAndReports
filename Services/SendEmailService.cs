using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;

namespace DocumentationAndReports.Services
{
    public class SendEmailService
    {
        private readonly string _apiKey;
        private readonly string _fromEmail;

        public SendEmailService(IConfiguration configuration)
        {
            _apiKey = configuration["ApiKey"];
            _fromEmail = configuration["SenderEmail"];
        }

        public async Task SendEmailAsync(string to, string subject, string text)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail);
            var toEmail = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, text, null);

            var response = await client.SendEmailAsync(msg);
            var responseBody = await response.Body.ReadAsStringAsync();

            Console.WriteLine($"SendGrid Response: {response.StatusCode}");
            Console.WriteLine($"SendGrid Response Body: {responseBody}");
        }
    }
}
