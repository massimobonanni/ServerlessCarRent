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
    public class ReturnCarClient
    {
        private readonly ILogger<ReturnCarClient> _logger;

        public ReturnCarClient(ILogger<ReturnCarClient> logger)
        {
            _logger = logger;
        }

        [OpenApiOperation(operationId: "returnCar", ["Rentals Management"],
            Summary = "Return a car", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ReturnCarRequest),
            Description = "Info about the car to return.", Required = true)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json",
            typeof(ReturnCarResponse), Summary = "Return operation response.",
            Description = "If the request is valid, the response contains the info of the closed rent.")]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, "text/plain", typeof(string),
            Summary = "The request is not valid. The API returns a message with the error")]
        [OpenApiResponseWithoutBody(HttpStatusCode.NotFound,
            Summary = "The car is not exists")]

        [Function("ReturnCar")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "rents/{carPlate}")] HttpRequest req,
            string carPlate,
            [DurableClient] DurableTaskClient client)
        {
            _logger.LogInformation("ReturnCar function");

            try
            {
                string payloadContent = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonConvert.DeserializeObject<ReturnCarRequest>(payloadContent);

                if (request == null)
                    return new BadRequestObjectResult("The request is not valid");

                var car = await client.Entities.GetCarDataAsync(carPlate);

                if (car == null)
                    return new NotFoundObjectResult("The car is not exist");

                if (!car.CanBeReturn())
                    return new BadRequestObjectResult("The car cannot be returned");

                var orchestrationDto = new ReturnOrchestratorDto()
                {
                    CarPlate = carPlate,
                    RentalEndDate = request.RentalEndDate
                };

                var returnTaskName = new TaskName(nameof(ReturnOrchestrator));
                var returnOperationId = await client.ScheduleNewOrchestrationInstanceAsync(returnTaskName,
                    orchestrationDto);

                var waitForCompletionToken = new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token;
                await client.WaitForInstanceCompletionAsync(returnOperationId, true, waitForCompletionToken);

                var response = new ReturnCarResponse()
                {
                    CarPlate = carPlate,
                    ReturnOperationStatus = ReturnOperationState.Pending
                };

                var orchestratorStatus = await client.GetInstanceAsync(returnOperationId, true);
                if (orchestratorStatus.RuntimeStatus == OrchestrationRuntimeStatus.Completed)
                {
                    var orchestratorOutput = orchestratorStatus.ReadOutputAs<ReturnOrchestratorResponseDto>();
                    response.CostPerHour = orchestratorOutput.CostPerHour;
                    response.Cost = orchestratorOutput.Cost;
                    response.Currency = orchestratorOutput.Currency;
                    response.RentalId = orchestratorOutput.RentalId;
                    response.ReturnOperationStatus = orchestratorOutput.Status;
                }

                return response.CreateOkResponse();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
