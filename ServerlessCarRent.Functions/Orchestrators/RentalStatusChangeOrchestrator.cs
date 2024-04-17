using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServerlessCarRent.Common.Dtos;
using ServerlessCarRent.Functions.Activities;
using System.Threading.Tasks;

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
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            this._logger.LogInformation($"[START ORCHESTRATOR] --> {nameof(RentalStatusChangeOrchestrator)}");

            var orchestratorDto = context.GetInput<RentalStatusChangeOrchestratorDto>();

            await context.CallActivityAsync(nameof(SendEmailToAdminActivity), orchestratorDto);

            await context.CallActivityAsync(nameof(SendRentalStatusChangeNotificationToEventGridActivity), orchestratorDto);
        }


    }
}