using System.Threading.Tasks;
using DurableTask.Core.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Entities;
using Microsoft.Extensions.Logging;
using ServerlessCarRent.Common.Dtos;
using ServerlessCarRent.Common.Interfaces;
using ServerlessCarRent.Functions.Entities;

namespace ServerlessCarRent.Functions.Orchestrators
{
    public class RentOrchestrator
	{
		private readonly ILogger<RentOrchestrator> _logger;

		public RentOrchestrator(ILogger<RentOrchestrator> logger)
		{
			_logger = logger;
		}

		[Function(nameof(RentOrchestrator))]
		public async Task<RentOrchestratorResponseDto> RunOrchestrator(
			[OrchestrationTrigger] TaskOrchestrationContext context)
		{
            this._logger.LogInformation($"[START ORCHESTRATOR] --> {nameof(RentOrchestrator)}");

            var requestDto = context.GetInput<RentOrchestratorDto>();

			var entityId = new EntityInstanceId(nameof(PickupLocationEntity), requestDto.PickupLocation);

			var rentCarDto = new RentCarPickupLocationDto()
			{
				RentalId = context.InstanceId,
				CarPlate = requestDto.CarPlate,
				RentalStart = requestDto.RentalStartDate,
				RenterFirstName = requestDto.RenterFirstName,
				RenterLastName = requestDto.RenterLastName,
				RenterEmail= requestDto.RenterEmail,
			};

			var rentOperationresult = await context.Entities.CallEntityAsync<bool>(entityId,
				nameof(IPickupLocationEntity.RentCar), rentCarDto);

			var response = new RentOrchestratorResponseDto()
			{
				CarPlate=requestDto.CarPlate,
				PickupLocation=requestDto.PickupLocation,
				RentalId=context.InstanceId,
				Status= rentOperationresult ? RentOperationState.Complete : RentOperationState.Error
			};
			return response;
		}


	}
}