using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Client.Entities;
using Microsoft.Extensions.Logging;
using ServerlessCarRent.Functions.Responses;
using System.Net;

namespace ServerlessCarRent.Functions.Clients
{
    public class GetPickupLocationClient
    {
        private readonly ILogger<GetPickupLocationClient> _logger;

        public GetPickupLocationClient(ILogger<GetPickupLocationClient> logger)
        {
            _logger = logger;
        }

        [OpenApiOperation(operationId: "getPickupLocation", ["Pickup Locations Management"],
           Summary = "Retrieve the information about a pickup location", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("identifier", Summary = "The identifier of the pickup location to retrieve",
           In = Microsoft.OpenApi.Models.ParameterLocation.Path, Required = true,
           Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json",
            typeof(GetPickupLocationResponse), Summary = "The pickup location details",
            Description = "If the pickup location exists, the response contains the detail of the pickup location.")]
        [OpenApiResponseWithoutBody(HttpStatusCode.NotFound,
           Summary = "The pickup location with the search identifier doesn't exist")]

        [Function("GetPickupLocation")]
        public async Task<IActionResult> Run(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "pickuplocations/{identifier}")] HttpRequest req,
           string identifier,
           [DurableClient] DurableTaskClient client)
        {
            _logger.LogInformation("GetPickupLocation function");

            try
            {
                var locationData = await client.Entities.GetPickupLocationDataAsync(identifier);
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

