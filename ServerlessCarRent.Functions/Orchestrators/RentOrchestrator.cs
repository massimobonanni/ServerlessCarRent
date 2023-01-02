using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using ServerlessCarRent.Common.Dtos;
using ServerlessCarRent.Common.Interfaces;
using ServerlessCarRent.Functions.Clients;
using ServerlessCarRent.Functions.Entities;

namespace ServerlessCarRent.Functions.Orchestrators
{
	public class RentOrchestrator
	{
		private readonly ILogger _logger;

		public RentOrchestrator(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<RentOrchestrator>();
		}

		[FunctionName("RentOrchestrator")]
		public async Task<RentOrchestratorResponseDto> RunOrchestrator(
			[OrchestrationTrigger] IDurableOrchestrationContext context)
		{
			var requestDto = context.GetInput<RentOrchestratorDto>();

			var entityId = new EntityId(nameof(PickupLocationEntity), requestDto.PickupLocation);

			var rentCarDto = new RentCarPickupLocationDto()
			{
				RentalId = context.InstanceId,
				CarPlate = requestDto.CarPlate,
				RentalStart = requestDto.RentalStartDate,
				RenterFirstName = requestDto.RenterFirstName,
				RenterLastName = requestDto.RenterLastName
			};

			var rentOperationresult = await context.CallEntityAsync<bool>(entityId,
				nameof(IPickupLocationEntity.RentCar), rentCarDto);

			var response = new RentOrchestratorResponseDto()
			{
				CarPlate=requestDto.CarPlate,
				PickupLocation=requestDto.PickupLocation,
				RentId=context.InstanceId,
				Status= rentOperationresult ? RentOperationState.Complete : RentOperationState.Error
			};
			return response;
		}


	}
}