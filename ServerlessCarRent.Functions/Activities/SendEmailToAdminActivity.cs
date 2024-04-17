﻿using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServerlessCarRent.Common.Dtos;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;

namespace ServerlessCarRent.Functions.Activities
{
    public class SendEmailToAdminActivity
    {
        private readonly ILogger<SendEmailToAdminActivity> _logger;
        private readonly IConfiguration _configuration;

        public SendEmailToAdminActivity(IConfiguration configuration,
            ILogger<SendEmailToAdminActivity> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [FunctionName(nameof(SendEmailToAdminActivity))]
        [SendGridOutput(ApiKey = "SendGridApiKey")]
        public async Task<SendGridMessage> Run([ActivityTrigger] RentalStatusChangeOrchestratorDto context)
        {
            this._logger.LogInformation($"[START ACTIVITY] --> {nameof(SendEmailToAdminActivity)}");

            var adminEmail = this._configuration.GetValue<string>("AdminEmail");

            if (!string.IsNullOrWhiteSpace(adminEmail))
            {
                var message = await CreateSendGridMessageAsync(adminEmail, context);
                return message;
            }
            else
            {
                this._logger.LogWarning("AdminEmail not setted in the configuration file. The Sendmail activity cannot send email to admin.");
                return null;
            }

        }

        private async Task<SendGridMessage> CreateSendGridMessageAsync(string toEmail,
            RentalStatusChangeOrchestratorDto context)
        {
            var message = new SendGridMessage()
            {
                Subject = $"Rental state changed for car {context.CarPlate}",
                From = new EmailAddress("noreply@serverlesscarrent.com")
            };

            message.AddTo(new EmailAddress(toEmail));

            var contextJson = JsonConvert.SerializeObject(context, Formatting.Indented);
            byte[] byteArray = Encoding.ASCII.GetBytes(contextJson);
            MemoryStream stream = new MemoryStream(byteArray);
            await message.AddAttachmentAsync("payload.json", stream);

            var strBuilder = new StringBuilder();
            strBuilder.AppendLine("<html><body>");
            strBuilder.AppendLine($"<h1>Car: [{context.CarPlate}] - {context.CarModel}</h1>");
            strBuilder.AppendLine($"<p>rental status: {context.NewRentalStatus}</p>");
            strBuilder.AppendLine($"<p>Car rented by: {context.CurrentRenter?.LastName} {context.CurrentRenter?.FirstName} ({context.CurrentRenter?.Email})</p>");
            strBuilder.AppendLine("</body></html>");

            message.HtmlContent = strBuilder.ToString();

            return message;
        }
    }
}
