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

namespace ServerlessCarRent.Functions.Clients
{
    public class InitializePickupLocationClient
    {
        private readonly ILogger _logger;

        public InitializePickupLocationClient(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<InitializePickupLocationClient>();
        }

        [OpenApiOperation(operationId: "initializePickupLocation", new[] { "Pickup Locations Management" },
            Summary = "Create and initialize a new pickup location", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(InitializePickupLocationRequest),
            Description = "Info about the pickup location to create.", Required = true)]
        [OpenApiResponseWithBody(HttpStatusCode.Created, "application/json",
            typeof(InitializePickupLocationResponse), Summary = "New pickup location response.",
            Description = "If the request is valid, the response contains the info of the pickup location created.")]
        [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest,
            Summary = "The request is not valid because city or location is not valid")]
        [OpenApiResponseWithoutBody(HttpStatusCode.Conflict, Summary = "A pickup location with the same identifier already exists")]

        [FunctionName("InitializePickupLocation")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "pickuplocations")] HttpRequest req,
            [DurableClient] IDurableEntityClient client)
        {
            _logger.LogInformation("InitializePickupLocation function");

            try
            {
                string payloadContent = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonConvert.DeserializeObject<InitializePickupLocationRequest>(payloadContent);

                if (request == null || !request.IsValid())
                    return new BadRequestObjectResult("The pickup location info are not valid");

                if (await client.PickupLocationExistsAsync(request.Identifier))
                    return new ConflictResult(); ;

                var entityId = new EntityId(nameof(PickupLocationEntity), request.Identifier);
                var pickupLocationDto = new InitializePickupLocationDto()
                {
                    Location = request.Location,
                    Status = request.Status,
                    City = request.City,
                };

                await client.SignalEntityAsync<IPickupLocationEntity>(entityId, e => e.Initialize(pickupLocationDto));

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
