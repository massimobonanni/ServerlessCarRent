using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ServerlessCarRent.Common.Dtos;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Azure.Messaging.EventGrid;
using Microsoft.Identity.Client;

namespace ServerlessCarRent.Functions.Activities
{
    public class SendRentalStatusChangeNotificationToEventGridActivity
    {
        private readonly ILogger<SendRentalStatusChangeNotificationToEventGridActivity> _logger;
        private readonly IConfiguration _configuration;

        public SendRentalStatusChangeNotificationToEventGridActivity(IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<SendRentalStatusChangeNotificationToEventGridActivity>();
        }

        [FunctionName(nameof(SendRentalStatusChangeNotificationToEventGridActivity))]
        public async Task Run([ActivityTrigger] RentalStatusChangeOrchestratorDto context,
            [EventGrid(TopicEndpointUri = "TopicEndpoint", TopicKeySetting = "TopicKey")] IAsyncCollector<EventGridEvent> eventCollector)
        {
            this._logger.LogInformation($"[START ACTIVITY] --> {nameof(SendRentalStatusChangeNotificationToEventGridActivity)}");

            var @event = new EventGridEvent(
              subject: $"cars/{context.CarPlate}",
              eventType: "RentalStatusChanged",
              dataVersion: "1.0",
              data: context);

            await eventCollector.AddAsync(@event);

            this._logger.LogInformation("Event sended to custom topic", JsonConvert.SerializeObject(@event));

        }
    }
}
