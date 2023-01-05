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
    public class UpdatePickupLocationClient
    {
        private readonly ILogger _logger;

        public UpdatePickupLocationClient(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<UpdateCarClient>();
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
            IActionResult responseData = null;

            try
            {
                string payloadContent = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonConvert.DeserializeObject<UpdatePickupLocationRequest>(payloadContent);

                if (request != null)
                {
                    if (await client.PickupLocationExistsAsync(id))
                    {
                        var entityId = new EntityId(nameof(PickupLocationEntity), id);
                        var locationDto = new UpdatePickupLocationDto()
                        {
                            City = request.City,
                            Location = request.Location,
                            Status = request.Status,
                        };

                        await client.SignalEntityAsync<IPickupLocationEntity>(entityId,
                            e => e.Update(locationDto));


                        responseData = new NoContentResult();
                    }
                    else
                    {
                        responseData = new NotFoundResult();
                    }
                }
                else
                {
                    responseData = new BadRequestResult();
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
