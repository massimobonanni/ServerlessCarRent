using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServerlessCarRent.Common.Dtos;
using ServerlessCarRent.Common.Interfaces;
using ServerlessCarRent.Functions.Activities;
using ServerlessCarRent.Functions.Clients;
using ServerlessCarRent.Functions.Entities;

namespace ServerlessCarRent.Functions.Orchestrators
{
    public class RentalStatusChangeOrchestrator
    {
        private readonly ILogger<RentalStatusChangeOrchestrator> _logger;
        private readonly IConfiguration _configuration;

        public RentalStatusChangeOrchestrator(IConfiguration configuration,
            ILogger<RentalStatusChangeOrchestrator> logger)
        {
            _configuration= configuration;
            _logger = logger;
        }

        [FunctionName(nameof(RentalStatusChangeOrchestrator))]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            this._logger.LogInformation($"[START ORCHESTRATOR] --> {nameof(RentalStatusChangeOrchestrator)}");

            var orchestratorDto = context.GetInput<RentalStatusChangeOrchestratorDto>();

            await context.CallActivityAsync(nameof(SendEmailToAdminActivity), orchestratorDto);

            await context.CallActivityAsync(nameof(SendRentalStatusChangeNotificationToEventGridActivity), orchestratorDto);
        }


    }
}