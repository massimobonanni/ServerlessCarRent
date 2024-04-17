using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using ServerlessCarRent.Functions.Requests;
using ServerlessCarRent.Functions.Entities;
using ServerlessCarRent.Common.Dtos;
using ServerlessCarRent.Common.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Client.Entities;
using Microsoft.DurableTask.Entities;

namespace ServerlessCarRent.Functions.Clients
{
    public class UpdateCarClient
    {
        private readonly ILogger<UpdateCarClient> _logger;

        public UpdateCarClient(ILogger<UpdateCarClient> logger)
        {
            _logger = logger;
        }

        [OpenApiOperation(operationId: "updateCar", ["Cars Management"],
            Summary = "Update info for an existing car", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UpdateCarRequest),
            Description = "Info about the car to update.", Required = true)]
        [OpenApiResponseWithoutBody(HttpStatusCode.NoContent, Summary = "Car updated.")]
        [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest,
            Summary = "The request is not valid because one of the plate or model is not valid")]
        [OpenApiResponseWithoutBody(HttpStatusCode.NotFound, Summary = "The car doesn't exist")]

        [Function("UpdateCar")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "cars/{carPlate}")] HttpRequest req,
            string carPlate,
            [DurableClient] DurableTaskClient client)
        {
            _logger.LogInformation("UpdateCar function");

            try
            {
                string payloadContent = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonConvert.DeserializeObject<UpdateCarRequest>(payloadContent);

                if (request == null)
                    return new BadRequestObjectResult("The update info are not valid");

                if (!await client.Entities.CarExistsAsync(carPlate))
                    return new NotFoundObjectResult("The car is not exist");


                var entityId = new EntityInstanceId(nameof(CarEntity), carPlate);
                var carDto = new UpdateCarDto()
                {
                    NewCarStatus = request.NewCarStatus,
                    NewCostPerHour = request.NewCostPerHour,
                    NewCurrency = request.NewCurrency,
                    NewPickupLocation = request.NewPickupLocation
                };

                await client.Entities.SignalEntityAsync(entityId, 
                    nameof(CarEntity.Update),
                    carDto);

                return new NoContentResult();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
