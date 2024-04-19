using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Client.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServerlessCarRent.Common.Dtos;
using ServerlessCarRent.Functions.Orchestrators;
using ServerlessCarRent.Functions.Requests;
using ServerlessCarRent.Functions.Responses;
using System.Net;

namespace ServerlessCarRent.Functions.Clients
{
    public class RentCarClient
    {
        private readonly ILogger<RentCarClient> _logger;

        public RentCarClient(ILogger<RentCarClient> logger)
        {
            _logger = logger;
        }

        [OpenApiOperation(operationId: "rentCar", ["Rentals Management"],
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

        [Function("RentCar")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "rents")] HttpRequest req,
            [DurableClient] DurableTaskClient client)
        {
            _logger.LogInformation("RentCar function");

            try
            {
                string payloadContent = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonConvert.DeserializeObject<RentCarRequest>(payloadContent);

                if (request == null || !request.IsValid())
                    return new BadRequestObjectResult("The rent car info are not valid");

                var car = await client.Entities.GetCarDataAsync(request.CarPlate);

                if (car == null)
                    return new NotFoundObjectResult("The car is not exist");

                if (!car.CanBeRent())
                    new BadRequestObjectResult("The car cannot be rented");

                var location = await client.Entities.GetPickupLocationDataAsync(car.PickupLocation);

                if (location.Status != Common.Models.PickupLocation.PickupLocationState.Open)
                    return new BadRequestObjectResult("The pickup location is not open");

                if (car.PickupLocation != request.PickupLocation)
                    return new BadRequestObjectResult("The car location is not the same of the rent location");

                var orchestrationDto = new RentOrchestratorDto()
                {
                    CarPlate = request.CarPlate,
                    PickupLocation = request.PickupLocation,
                    RentalStartDate = request.RentalStartDate,
                    RenterFirstName = request.RenterFirstName,
                    RenterLastName = request.RenterLastName,
                    RenterEmail = request.RenterEmail,
                };

                var rentTaskName = new TaskName(nameof(RentOrchestrator));
                var rentOperationId = await client.ScheduleNewOrchestrationInstanceAsync(rentTaskName, orchestrationDto);

                var waitForCompletionToken = new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token;
                await client.WaitForInstanceCompletionAsync(rentOperationId, true, waitForCompletionToken);

                var response = new RentCarResponse()
                {
                    CarPlate = request.CarPlate,
                    PickupLocation = request.PickupLocation,
                    RentalId = rentOperationId,
                    RentOperationStatus = RentOperationState.Pending
                };

                var orchestratorStatus = await client.GetInstanceAsync(rentOperationId,true);
                if (orchestratorStatus.RuntimeStatus == OrchestrationRuntimeStatus.Completed)
                {
                    var orchestratorOutput = orchestratorStatus.ReadOutputAs<RentOrchestratorResponseDto>();
                    response.RentOperationStatus = orchestratorOutput.Status;
                }

                return response.CreateOkResponse();

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "RentCar");
                throw;
            }
        }
    }
}
