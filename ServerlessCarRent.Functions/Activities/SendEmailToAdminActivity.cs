using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServerlessCarRent.Common.Dtos;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;
using ServerlessCarRent.Functions.Utilities;

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

        [Function(nameof(SendEmailToAdminActivity))]
        [SendGridOutput(ApiKey = "SendGridApiKey")]
        public async Task<SendGridMessage> Run([ActivityTrigger] RentalStatusChangeOrchestratorDto context)
        {
            this._logger.LogInformation($"[START ACTIVITY] --> {nameof(SendEmailToAdminActivity)}");

            var senderEmail = this._configuration.GetValue<string>("FromEmail");
            var adminEmail = this._configuration.GetValue<string>("AdminEmail");

            if (!string.IsNullOrWhiteSpace(adminEmail))
            {
                var message = await CreateSendGridMessageAsync(senderEmail,adminEmail, context);
                return message;
            }
            else
            {
                this._logger.LogWarning("AdminEmail not setted in the configuration file. The Sendmail activity cannot send email to admin.");
                return null;
            }

        }

        private async Task<SendGridMessage> CreateSendGridMessageAsync(string senderEmail,string toEmail,
            RentalStatusChangeOrchestratorDto context)
        {
            var subject = $"Rental state changed for car {context.CarPlate}";
            var from = new EmailAddress(senderEmail);
            var to = new EmailAddress(toEmail);
            var textContent= MailUtilities.GenerateHtmlContentForAdmin(context);
            var htmlContent= MailUtilities.GenerateTextContentForAdmin(context);

            var message = MailHelper.CreateSingleEmail(from, to, subject, textContent, htmlContent);

            var contextJson = JsonConvert.SerializeObject(context, Formatting.Indented);
            byte[] byteArray = Encoding.ASCII.GetBytes(contextJson);
            MemoryStream stream = new MemoryStream(byteArray);
            await message.AddAttachmentAsync("payload.json", stream);
           
            return message;
        }
    }
}
