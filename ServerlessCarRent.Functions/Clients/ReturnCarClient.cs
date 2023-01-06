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
    public class ReturnCarClient
    {
        private readonly ILogger _logger;

        public ReturnCarClient(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ReturnCarClient>();
        }

        [OpenApiOperation(operationId: "returnCar", new[] { "Rentals Management" },
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

        [FunctionName("ReturnCar")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "rents/{carPlate}")] HttpRequest req,
            string carPlate,
            [DurableClient] IDurableClient client)
        {
            _logger.LogInformation("ReturnCar function");

            try
            {
                string payloadContent = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonConvert.DeserializeObject<ReturnCarRequest>(payloadContent);

                if (request == null)
                    return new BadRequestObjectResult("The request is not valid");

                var car = await client.GetCarDataAsync(carPlate);

                if (car == null)
                    return new NotFoundObjectResult("The car is not exist");

                if (!car.CanBeReturn())
                    return new BadRequestObjectResult("The car cannot be returned");

                var orchestrationDto = new ReturnOrchestratorDto()
                {
                    CarPlate = carPlate,
                    RentalEndDate = request.RentalEndDate
                };

                var returnOperationId = await client.StartNewAsync(nameof(ReturnOrchestrator), orchestrationDto);

                await client.WaitForCompletionOrCreateCheckStatusResponseAsync(req, returnOperationId,
                    TimeSpan.FromSeconds(10));

                var response = new ReturnCarResponse()
                {
                    CarPlate = carPlate,
                    ReturnOperationStatus = ReturnOperationState.Pending
                };

                var orchestratorStatus = await client.GetStatusAsync(returnOperationId);
                if (orchestratorStatus.RuntimeStatus == OrchestrationRuntimeStatus.Completed)
                {
                    var orchestratorOutput = orchestratorStatus.Output.ToObject<ReturnOrchestratorResponseDto>();
                    response.CostPerHour = orchestratorOutput.CostPerHour;
                    response.Cost = orchestratorOutput.Cost;
                    response.Currency = orchestratorOutput.Currency;
                    response.RentalId = orchestratorOutput.RentalId;
                    response.ReturnOperationStatus = orchestratorOutput.Status;
                }

                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
