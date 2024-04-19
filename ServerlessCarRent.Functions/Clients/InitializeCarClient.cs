using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Client.Entities;
using Microsoft.DurableTask.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServerlessCarRent.Common.Dtos;
using ServerlessCarRent.Common.Interfaces;
using ServerlessCarRent.Functions.Entities;
using ServerlessCarRent.Functions.Requests;
using ServerlessCarRent.Functions.Responses;
using System.Net;

namespace ServerlessCarRent.Functions.Clients
{
    public class InitializeCarClient
    {
        private readonly ILogger<InitializeCarClient> _logger;

        public InitializeCarClient(ILogger<InitializeCarClient> logger)
        {
            _logger = logger;
        }

        [OpenApiOperation(operationId: "initializeCar", ["Cars Management"],
            Summary = "Create and initialize a new car", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(InitializeCarRequest),
            Description = "Info about the car to create.", Required = true)]
        [OpenApiResponseWithBody(HttpStatusCode.Created, "application/json",
            typeof(InitializeCarResponse), Summary = "New car response.",
            Description = "If the request is valid, the response contains the info of the car created.")]
        [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest,
            Summary = "The request is not valid because one of the plate or model is not valid")]
        [OpenApiResponseWithoutBody(HttpStatusCode.Conflict, Summary = "A car with the same plate already exists")]

        [Function("InitializeCar")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "cars")] HttpRequest req,
            [DurableClient] DurableTaskClient client)
        {
            _logger.LogInformation("InitializeCar function");

            try
            {
                string payloadContent = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonConvert.DeserializeObject<InitializeCarRequest>(payloadContent);

                if (request == null || !request.IsValid())
                    return new BadRequestObjectResult("The car info are not valid");

                if (await client.Entities.CarExistsAsync(request.Plate))
                    return new ConflictResult();

                var entityId = new EntityInstanceId(nameof(CarEntity), request.Plate);
                var carDto = new InitializeCarDto()
                {
                    Model = request.Model,
                    PickupLocation = request.PickupLocation,
                    CarStatus = request.CurrentStatus,
                    CostPerHour = request.CostPerHour,
                    Currency = request.Currency,
                };

                await client.Entities.SignalEntityAsync(entityId,
                    nameof(CarEntity.Initialize),
                    carDto);

                var response = new InitializeCarResponse()
                {
                    Plate = request.Plate
                };

                return response.CreateOkResponse(201);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
