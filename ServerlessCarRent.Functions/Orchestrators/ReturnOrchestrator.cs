using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Entities;
using Microsoft.Extensions.Logging;
using ServerlessCarRent.Common.Dtos;
using ServerlessCarRent.Common.Interfaces;
using ServerlessCarRent.Functions.Entities;

namespace ServerlessCarRent.Functions.Orchestrators
{
    public class ReturnOrchestrator
    {
        private readonly ILogger<ReturnOrchestrator> _logger;

        public ReturnOrchestrator(ILogger<ReturnOrchestrator> logger)
        {
            _logger = logger;
        }

        [Function(nameof(ReturnOrchestrator))]
        public async Task<ReturnOrchestratorResponseDto> RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            this._logger.LogInformation($"[START ORCHESTRATOR] --> {nameof(ReturnOrchestrator)}");
            var requestDto = context.GetInput<ReturnOrchestratorDto>();

            var entityId = new EntityInstanceId(nameof(CarEntity), requestDto.CarPlate);

            var rentCarDto = new ReturnCarDto()
            {
                EndDate = requestDto.RentalEndDate,
            };

            var returnOperationresult = await context.Entities.CallEntityAsync<ReturnCarResponseDto>(entityId,
                nameof(ICarEntity.Return), rentCarDto);

            var response = new ReturnOrchestratorResponseDto()
            {
                CarPlate = requestDto.CarPlate,
                Cost = returnOperationresult.Cost,
                CostPerHour = returnOperationresult.CostPerHour,
                Currency = returnOperationresult.Currency,
                RentalId = returnOperationresult.RentalId,
                Status = returnOperationresult.Succeeded ? ReturnOperationState.Complete : ReturnOperationState.Error
            };

            return response;
        }


    }
}