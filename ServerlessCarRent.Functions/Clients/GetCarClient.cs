using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Client.Entities;
using Microsoft.Extensions.Logging;
using ServerlessCarRent.Functions.Responses;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using System.Net;

namespace ServerlessCarRent.Functions.Clients
{
    public class GetCarClient
    {
        private readonly ILogger<GetCarClient> _logger;

        public GetCarClient(ILogger<GetCarClient> logger)
        {
            _logger = logger;
        }


        [OpenApiOperation(operationId: "getCar", ["Cars Management"],
           Summary = "Retrieve the information about a car", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("plate", Summary = "The plate of the car to retrieve",
           In = Microsoft.OpenApi.Models.ParameterLocation.Path, Required = true,
           Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("details", Summary = "Set this parameter if you also want rentals details",
           In = Microsoft.OpenApi.Models.ParameterLocation.Query, Required = false,
           Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json",
            typeof(GetCarResponse), Summary = "The car details",
            Description = "If the car exists, the response contains the detail of the car.")]
        [OpenApiResponseWithoutBody(HttpStatusCode.NotFound,
           Summary = "The car with the search plate doesn't exist")]

        [Function("GetCar")]
        public async Task<IActionResult> Run(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "cars/{plate}")] HttpRequest req,
           string plate,
           [DurableClient] DurableTaskClient client)
        {
            _logger.LogInformation("GetCar function");
            // Get the details parameter
            var details = req.Query.Any(e => e.Key.Equals("details", StringComparison.CurrentCultureIgnoreCase));

            try
            {
                // Get the car data
                var carData = await client.Entities.GetCarDataAsync(plate);
                if (carData == null)
                    return new NotFoundResult(); ;

                var response = new GetCarResponse()
                {
                    Plate = plate,
                    Model = carData.Model,
                    PickupLocation = carData.PickupLocation,
                    CurrentRentalState = carData.CurrentRentalState,
                    CurrentState = carData.CurrentState,
                    Currency = carData.Currency,
                    CostPerHour = carData.CostPerHour,
                    CurrentRenterEmail = carData.CurrentRenter?.Email,
                    CurrentRenterFirstName = carData.CurrentRenter?.FirstName,
                    CurrentRenterLastName = carData.CurrentRenter?.LastName,
                };

                if (details)
                {
                    // Get the car rentals data
                    var carRentalsData = await client.Entities.GetCarRentalsDataAsync(plate);
                    if (carRentalsData != null)
                    {
                        response.Rentals = carRentalsData.Rentals
                            .Select(r =>
                                new GetCarResponse.CarRentalDto()
                                {
                                    RentalId = r.Rental.Id,
                                    CostPerHour = r.CostPerHour,
                                    Currency = r.Currency,
                                    RentalStart = r.Rental.StartDate,
                                    RentalEnd = r.Rental.EndDate,
                                    RenterFirstName = r.Renter.FirstName,
                                    RenterLastName = r.Renter.LastName,
                                    RenterEmail = r.Renter.Email,
                                    Cost = r.TotalCost
                                })
                            .ToList();
                    }
                }

                return response.CreateOkResponse();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}

