using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using ServerlessCarRent.Common.Dtos;
using Azure.Messaging.EventGrid;
using Microsoft.Azure.Functions.Worker;
using System.Text.Json;

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

        [Function(nameof(SendRentalStatusChangeNotificationToEventGridActivity))]
        [EventGridOutput(TopicEndpointUri = "TopicEndpoint", TopicKeySetting = "TopicKey")]
        public EventGridEvent Run([ActivityTrigger] RentalStatusChangeOrchestratorDto context)
        {
            this._logger.LogInformation($"[START ACTIVITY] --> {nameof(SendRentalStatusChangeNotificationToEventGridActivity)}");

            var @event = new EventGridEvent($"cars/{context.CarPlate}",
              "RentalStatusChanged","1.0", context);

            this._logger.LogInformation("Event sending to custom topic", JsonConvert.SerializeObject(@event));

            return @event;

        }
    }
}
