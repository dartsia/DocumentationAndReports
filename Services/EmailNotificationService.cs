using Npgsql;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace DocumentationAndReports.Services
{
    public class EmailNotificationService: BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public EmailNotificationService(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connString = _configuration.GetConnectionString("DefaultConnection");

            await using var conn = new NpgsqlConnection(connString);
            await conn.OpenAsync(stoppingToken);

            conn.Notification += async (o, e) =>
            {
                try
                {
                    var payload = JsonSerializer.Deserialize<StateChangePayload>(e.Payload);
                    using var scope = _serviceProvider.CreateScope();
                    var emailService = scope.ServiceProvider.GetRequiredService<SendEmailService>();

                    var subject = $"Зміна стану військовослужбовця: {payload.Name}";
                    var text = $"Стан військовослужбовця {payload.Name} змінився на \"{payload.State}\".";

                    await emailService.SendEmailAsync(payload.RelativesEmail, subject, text);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending email: {ex.Message}");
                }
            };

            using (var listenCmd = conn.CreateCommand())
            {
                listenCmd.CommandText = "LISTEN state_changes;";
                await listenCmd.ExecuteNonQueryAsync(stoppingToken);
            }

            Console.WriteLine("Listening for state_changes...");

            while (!stoppingToken.IsCancellationRequested)
            {
                await conn.WaitAsync(stoppingToken);
            }
        }

        private class StateChangePayload
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
            [JsonPropertyName("state")]
            public string State { get; set; }
            [JsonPropertyName("relatives_email")]
            public string RelativesEmail { get; set; }
        }
    }
}
