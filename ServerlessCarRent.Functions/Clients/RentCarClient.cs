using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServerlessCarRent.Functions.Requests;
using ServerlessCarRent.Functions.Responses;
using ServerlessCarRent.Functions.Entities;
using ServerlessCarRent.Common.Dtos;
using ServerlessCarRent.Common.Interfaces;
using ServerlessCarRent.Functions.Orchestrators;

namespace ServerlessCarRent.Functions.Clients
{
	public class RentCarClient
	{
		private readonly ILogger _logger;

		public RentCarClient(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<RentCarClient>();
		}

		[OpenApiOperation(operationId: "rentCar", new[] { "Rents Management" },
			Summary = "Rent a car", Visibility = OpenApiVisibilityType.Important)]
		[OpenApiRequestBody(contentType: "application/json", bodyType: typeof(RentCarRequest),
			Description = "Info about the rent.", Required = true)]
		[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json",
			typeof(RentCarResponse), Summary = "Rent operation response.",
			Description = "If the request is valid, the response contains the info of the rent.")]
		[OpenApiResponseWithBody(HttpStatusCode.BadRequest, "text/plain", typeof(string),
			Summary = "The request is not valid. The API returns a message with the error")]
		[OpenApiResponseWithoutBody(HttpStatusCode.NotFound,
			Summary = "The car is not exists")]

		[FunctionName("RentCar")]
		public async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "post", Route = "rents")] HttpRequest req,
			[DurableClient] IDurableClient client)
		{
			_logger.LogInformation("RentCar function");
			IActionResult responseData = null;

			try
			{
				string payloadContent = await new StreamReader(req.Body).ReadToEndAsync();
				var request = JsonConvert.DeserializeObject<RentCarRequest>(payloadContent);

				if (request != null && request.IsValid())
				{
					var car = await client.GetCarDataAsync(request.CarPlate);

					if (car != null)
					{
						if (car.CanBeRent())
						{
							if (car.PickupLocation == request.PickupLocation)
							{
								var orchestrationDto = new RentOrchestratorDto()
								{
									CarPlate = request.CarPlate,
									PickupLocation = request.PickupLocation,
									RentalStartDate = request.RentalStartDate,
									RenterFirstName = request.RenterFirstName,
									RenterLastName = request.RenterLastName,
								};

								var rentOperationId = await client.StartNewAsync(nameof(RentOrchestrator), orchestrationDto);

								await client.WaitForCompletionOrCreateCheckStatusResponseAsync(req, rentOperationId,
									TimeSpan.FromSeconds(10));

								var response = new RentCarResponse()
								{
									CarPlate = request.CarPlate,
									PickupLocation = request.PickupLocation,
									RentalId = rentOperationId,
									RentOperationStatus = RentOperationState.Pending
								};

								var orchestratorStatus = await client.GetStatusAsync(rentOperationId);
								if (orchestratorStatus.RuntimeStatus==OrchestrationRuntimeStatus.Completed)
								{
									var orchestratorOutput = orchestratorStatus.Output.ToObject<RentOrchestratorResponseDto>();
									response.RentOperationStatus = orchestratorOutput.Status;
								}

								responseData = new OkObjectResult(response);
							}
							else
							{
								responseData = new BadRequestObjectResult("The car location is not the same of the rent location");
							}
						}
						else
						{
							responseData = new BadRequestObjectResult("The car cannot be rented");
						}
					}
					else
					{
						responseData = new NotFoundResult();
					}
				}
				else
				{
					responseData = new BadRequestObjectResult("The car plate, location identifier or renter info are not valid");
				}
			}
			catch
			{
				responseData = new BadRequestResult();
			}

			return responseData;
		}
	}
}
