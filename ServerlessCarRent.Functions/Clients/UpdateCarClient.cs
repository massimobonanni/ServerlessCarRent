﻿using Microsoft.AspNetCore.Http;
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

namespace ServerlessCarRent.Functions.Clients
{
    public class UpdateCarClient
    {
        private readonly ILogger<UpdateCarClient> _logger;

        public UpdateCarClient(ILogger<UpdateCarClient> logger)
        {
            _logger = logger;
        }

        [OpenApiOperation(operationId: "updateCar", new[] { "Cars Management" },
            Summary = "Update info for an existing car", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UpdateCarRequest),
            Description = "Info about the car to update.", Required = true)]
        [OpenApiResponseWithoutBody(HttpStatusCode.NoContent, Summary = "Car updated.")]
        [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest,
            Summary = "The request is not valid because one of the plate or model is not valid")]
        [OpenApiResponseWithoutBody(HttpStatusCode.NotFound, Summary = "The car doesn't exist")]

        [FunctionName("UpdateCar")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "cars/{carPlate}")] HttpRequest req,
            string carPlate,
            [DurableClient] IDurableEntityClient client)
        {
            _logger.LogInformation("UpdateCar function");

            try
            {
                string payloadContent = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonConvert.DeserializeObject<UpdateCarRequest>(payloadContent);

                if (request == null)
                    return new BadRequestObjectResult("The update info are not valid");

                if (!await client.CarExistsAsync(carPlate))
                    return new NotFoundObjectResult("The car is not exist");


                var entityId = new EntityId(nameof(CarEntity), carPlate);
                var carDto = new UpdateCarDto()
                {
                    NewCarStatus = request.NewCarStatus,
                    NewCostPerHour = request.NewCostPerHour,
                    NewCurrency = request.NewCurrency,
                    NewPickupLocation = request.NewPickupLocation
                };

                await client.SignalEntityAsync<ICarEntity>(entityId, e => e.Update(carDto));

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
