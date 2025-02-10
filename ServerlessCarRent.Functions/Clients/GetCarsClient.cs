using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Client.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.CarRental;
using ServerlessCarRent.Functions.Entities;
using ServerlessCarRent.Functions.Responses;
using System.Net;
using static ServerlessCarRent.Functions.Responses.GetCarsResponse;

namespace ServerlessCarRent.Functions.Clients
{
    public class GetCarsClient
    {
        private readonly ILogger<GetCarsClient> _logger;

        public GetCarsClient(ILogger<GetCarsClient> logger)
        {
            _logger = logger;
        }

        [OpenApiOperation(operationId: "getCars", ["Cars Management"],
           Summary = "Search cars based on search parameters", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("plate", Summary = "The plate (or part of the plate) of the cars to search",
           In = Microsoft.OpenApi.Models.ParameterLocation.Query, Required = false,
           Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("model", Summary = "The model (or part of it) of the cars to search",
           In = Microsoft.OpenApi.Models.ParameterLocation.Query, Required = false,
           Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("location", Summary = "The location identifier (or part of it) of the cars to search",
           In = Microsoft.OpenApi.Models.ParameterLocation.Query, Required = false,
           Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("state", Summary = "The states of the car to search. this parameter can contain multiple states separated by '|'",
           In = Microsoft.OpenApi.Models.ParameterLocation.Query, Required = false,
           Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("rentalState", Summary = "The states of the rental for the car to search. this parameter can contain multiple states separated by '|'",
           In = Microsoft.OpenApi.Models.ParameterLocation.Query, Required = false,
           Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json",
            typeof(GetCarsResponse), Summary = "The result of the search")]

        [Function("GetCars")]
        public async Task<IActionResult> Run(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "cars")] HttpRequest req,
           [DurableClient] DurableTaskClient client)
        {
            _logger.LogInformation("GetCars function");
            IActionResult? responseData = null;

            var searchFilters = new GetCarsFilters(req);

            try
            {
                var response = new GetCarsResponse()
                {
                    LocationFilter = searchFilters.Location,
                    ModelFilter = searchFilters.Model,
                    PlateFilter = searchFilters.Plate,
                    RentalStatesFilter = searchFilters.RentalStates?.Select(s => s.ToString()),
                    StatesFilter = searchFilters.States?.Select(s => s.ToString()),
                    Cars = new List<GetCarsResponse.CarDto>()
                };

                EntityQuery queryDefinition = new EntityQuery()
                {
                    PageSize = 100,
                    IncludeState = true,
                    InstanceIdStartsWith = nameof(CarEntity),
                };

                var queryResult = client.Entities
                 .GetAllEntitiesAsync(queryDefinition)
                 .AsPages();

                await foreach (var page in queryResult)
                {
                    foreach (var item in page.Values)
                    {
                        var carDto = new CarDto(item.Id.Key, item.State.Value);

                        if (searchFilters.Match(carDto))
                            response.Cars.Add(carDto);
                    }
                }

                responseData = response.CreateOkResponse();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
                throw;
            }

            return responseData;
        }
    }

    internal class GetCarsFilters
    {
        public GetCarsFilters(HttpRequest req)
        {
            if (req.Query.ContainsKey("plate"))
                Plate = req.Query.GetFirstStringValue("plate")?.ToLower();
            if (req.Query.ContainsKey("model"))
                Model = req.Query.GetFirstStringValue("model")?.ToLower();
            if (req.Query.ContainsKey("location"))
                Location = req.Query.GetFirstStringValue("location")?.ToLower();
            if (req.Query.ContainsKey("state"))
            {
                var states = req.Query.GetFirstStringValue("state")?.ToLower()
                    .Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                States = states!.Select(s => (CarState)Enum.Parse(typeof(CarState), s, true));
            }
            if (req.Query.ContainsKey("rentalState"))
            {
                var rentalStates = req.Query.GetFirstStringValue("rentalState")?.ToLower()
                    .Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                RentalStates = rentalStates!.Select(s => (CarRentalState)Enum.Parse(typeof(CarRentalState), s, true));
            }
        }

        public bool Match(CarDto car)
        {
            bool result = true;

            if (!string.IsNullOrEmpty(Plate))
                result &= car.Plate!.Contains(Plate, StringComparison.CurrentCultureIgnoreCase);
            if (!string.IsNullOrEmpty(Model))
                result &= car.Model!.Contains(Model, StringComparison.CurrentCultureIgnoreCase);
            if (!string.IsNullOrEmpty(Location))
                result &= car.PickupLocation!.Contains(Location, StringComparison.CurrentCultureIgnoreCase);
            if (States != null && States.Any())
                result &= States.Contains(car.CurrentStatus);
            if (RentalStates != null && RentalStates.Any())
                result &= RentalStates.Contains(car.CurrentRentalStatus);

            return result;
        }

        public string? Plate { get; set; }
        public string? Model { get; set; }
        public string? Location { get; set; }
        public IEnumerable<CarState>? States { get; set; }
        public IEnumerable<CarRentalState>? RentalStates { get; set; }
    }
}

