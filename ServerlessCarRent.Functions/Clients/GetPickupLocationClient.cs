using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServerlessCarRent.Functions.Responses;

namespace ServerlessCarRent.Functions.Clients
{
    public class GetPickupLocationClient
    {
        private readonly ILogger _logger;

        public GetPickupLocationClient(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetPickupLocationClient>();
        }

        [OpenApiOperation(operationId: "getPickupLocation", new[] { "Pickup Locations Management" },
           Summary = "Retrieve the information about a pickup location", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("identifier", Summary = "The identifier of the pickup location to retrieve",
           In = Microsoft.OpenApi.Models.ParameterLocation.Path, Required = true,
           Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json",
            typeof(GetPickupLocationResponse), Summary = "The pickup location details",
            Description = "If the pickup location exists, the response contains the detail of the pickup location.")]
        [OpenApiResponseWithoutBody(HttpStatusCode.NotFound,
           Summary = "The pickup location with the search identifier doesn't exist")]

        [FunctionName("GetPickupLocation")]
        public async Task<IActionResult> Run(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "pickuplocations/{identifier}")] HttpRequest req,
           string identifier,
           [DurableClient] IDurableEntityClient client)
        {
            _logger.LogInformation("GetPickupLocation function");

            try
            {
                var locationData = await client.GetPickupLocationDataAsync(identifier);
                if (locationData == null)
                    return new NotFoundResult();


                var response = new GetPickupLocationResponse()
                {
                    Identifier = identifier,
                    City = locationData.City,
                    Location = locationData.Location,
                    Status = locationData.Status,
                    Cars = locationData.Cars
                        .Select(c => new GetPickupLocationResponse.PickupLocationCarDto()
                        {
                            Model = c.Model,
                            Plate = c.Plate,
                            RentalStatus = c.RentalStatus,
                            Status = c.Status
                        })
                        .ToList()
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

