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
    public class UpdatePickupLocationClient
    {
        private readonly ILogger<UpdatePickupLocationClient> _logger;

        public UpdatePickupLocationClient(ILogger<UpdatePickupLocationClient> logger)
        {
            _logger = logger;
        }

        [OpenApiOperation(operationId: "updatePickupLocation", new[] { "Pickup Locations Management" },
            Summary = "Update info for an existing pickup location", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UpdatePickupLocationRequest),
            Description = "Info about the pickup location to update.", Required = true)]
        [OpenApiResponseWithoutBody(HttpStatusCode.NoContent, Summary = "pickup location updated.")]
        [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest,
            Summary = "The request is not valid because the pickup location inf are not valid")]
        [OpenApiResponseWithoutBody(HttpStatusCode.NotFound, Summary = "The pickup location doesn't exist")]

        [FunctionName("UpdatePickupLocation")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "pickuplocations/{id}")] HttpRequest req,
            string id,
            [DurableClient] IDurableEntityClient client)
        {
            _logger.LogInformation("UpdatePickupLocation function");

            try
            {
                string payloadContent = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonConvert.DeserializeObject<UpdatePickupLocationRequest>(payloadContent);

                if (request == null)
                    return new BadRequestObjectResult("The pickup location update info are not valid");

                if (!await client.PickupLocationExistsAsync(id))
                    return new NotFoundObjectResult("The pickup location is not exist");

                var entityId = new EntityId(nameof(PickupLocationEntity), id);
                var locationDto = new UpdatePickupLocationDto()
                {
                    City = request.City,
                    Location = request.Location,
                    Status = request.Status,
                };

                await client.SignalEntityAsync<IPickupLocationEntity>(entityId, e => e.Update(locationDto));

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
