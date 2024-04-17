using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using ServerlessCarRent.Common.Dtos;
using Azure.Messaging.EventGrid;
using Microsoft.Azure.Functions.Worker;
using ActivityTriggerAttribute = Microsoft.Azure.Functions.Worker.ActivityTriggerAttribute;

namespace ServerlessCarRent.Functions.Activities
{
    public class SendRentalStatusChangeNotificationToEventGridActivity
    {
        private readonly ILogger<SendRentalStatusChangeNotificationToEventGridActivity> _logger;
        private readonly IConfiguration _configuration;

        public SendRentalStatusChangeNotificationToEventGridActivity(IConfiguration configuration,
            ILogger<SendRentalStatusChangeNotificationToEventGridActivity> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [FunctionName(nameof(SendRentalStatusChangeNotificationToEventGridActivity))]
        [EventGridOutput(TopicEndpointUri = "TopicEndpoint", TopicKeySetting = "TopicKey")]
        public EventGridEvent Run([ActivityTrigger] RentalStatusChangeOrchestratorDto context)
        {
            this._logger.LogInformation($"[START ACTIVITY] --> {nameof(SendRentalStatusChangeNotificationToEventGridActivity)}");

            var @event = new EventGridEvent(
              subject: $"cars/{context.CarPlate}",
              eventType: "RentalStatusChanged",
              dataVersion: "1.0",
              data: context);

            this._logger.LogInformation("Event sending to custom topic", JsonConvert.SerializeObject(@event));

            return @event;

        }
    }
}
