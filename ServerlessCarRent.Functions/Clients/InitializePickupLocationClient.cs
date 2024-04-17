using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
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
using Microsoft.DurableTask.Client;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client.Entities;
using Microsoft.DurableTask.Entities;

namespace ServerlessCarRent.Functions.Clients
{
    public class InitializePickupLocationClient
    {
        private readonly ILogger<InitializePickupLocationClient> _logger;

        public InitializePickupLocationClient(ILogger<InitializePickupLocationClient> logger)
        {
            _logger = logger;
        }

        [OpenApiOperation(operationId: "initializePickupLocation", ["Pickup Locations Management"],
            Summary = "Create and initialize a new pickup location", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(InitializePickupLocationRequest),
            Description = "Info about the pickup location to create.", Required = true)]
        [OpenApiResponseWithBody(HttpStatusCode.Created, "application/json",
            typeof(InitializePickupLocationResponse), Summary = "New pickup location response.",
            Description = "If the request is valid, the response contains the info of the pickup location created.")]
        [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest,
            Summary = "The request is not valid because city or location is not valid")]
        [OpenApiResponseWithoutBody(HttpStatusCode.Conflict, Summary = "A pickup location with the same identifier already exists")]

        [Function("InitializePickupLocation")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "pickuplocations")] HttpRequest req,
            [DurableClient] DurableTaskClient client)
        {
            _logger.LogInformation("InitializePickupLocation function");

            try
            {
                string payloadContent = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonConvert.DeserializeObject<InitializePickupLocationRequest>(payloadContent);

                if (request == null || !request.IsValid())
                    return new BadRequestObjectResult("The pickup location info are not valid");

                if (await client.Entities.PickupLocationExistsAsync(request.Identifier))
                    return new ConflictResult(); ;

                var entityId = new EntityInstanceId(nameof(PickupLocationEntity), request.Identifier);
                var pickupLocationDto = new InitializePickupLocationDto()
                {
                    Location = request.Location,
                    Status = request.Status,
                    City = request.City,
                };

                await client.Entities.SignalEntityAsync(entityId, 
                    nameof(PickupLocationEntity.Initialize),
                    pickupLocationDto);

                var response = new InitializePickupLocationResponse()
                {
                    Identifier = request.Identifier
                };

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
