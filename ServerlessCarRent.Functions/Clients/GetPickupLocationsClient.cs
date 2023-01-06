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
using ServerlessCarRent.Functions.Entities;
using ServerlessCarRent.Common.Models.Car;
using static ServerlessCarRent.Functions.Responses.GetCarsResponse;
using ServerlessCarRent.Common.Models.CarRental;
using ServerlessCarRent.Common.Models.PickupLocation;
using static ServerlessCarRent.Functions.Responses.GetPickupLocationsResponse;

namespace ServerlessCarRent.Functions.Clients
{
	public class GetPickupLocationsClient
	{
		private readonly ILogger _logger;

		public GetPickupLocationsClient(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<GetPickupLocationsClient>();
		}

		[OpenApiOperation(operationId: "getPickupLocations", new[] { "Pickup Locations Management" },
		   Summary = "Search pickup locations based on search parameters", Visibility = OpenApiVisibilityType.Important)]
		[OpenApiParameter("identifier", Summary = "The identifier (or part of the plate) of the locations to search",
		   In = Microsoft.OpenApi.Models.ParameterLocation.Query, Required = false,
		   Visibility = OpenApiVisibilityType.Important)]
		[OpenApiParameter("city", Summary = "The city (or part of it) of the locations to search",
		   In = Microsoft.OpenApi.Models.ParameterLocation.Query, Required = false,
		   Visibility = OpenApiVisibilityType.Important)]
		[OpenApiParameter("location", Summary = "The location description (or part of it) of the locations to search",
		   In = Microsoft.OpenApi.Models.ParameterLocation.Query, Required = false,
		   Visibility = OpenApiVisibilityType.Important)]
		[OpenApiParameter("state", Summary = "The states of the location to search. This parameter can contain multiple states separated by '|'",
		   In = Microsoft.OpenApi.Models.ParameterLocation.Query, Required = false,
		   Visibility = OpenApiVisibilityType.Important)]
		[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json",
			typeof(GetPickupLocationsResponse), Summary = "The result of the search")]

		[FunctionName("GetPickupLocations")]
		public async Task<IActionResult> Run(
		   [HttpTrigger(AuthorizationLevel.Function, "get", Route = "pickuplocations")] HttpRequest req,
		   [DurableClient] IDurableEntityClient client)
		{
			_logger.LogInformation("GetPickupLocations function");
			IActionResult responseData = null;

			var searchFilters = new GetPickupLocationsFilters(req);

			try
			{
				var response = new GetPickupLocationsResponse()
				{
					LocationFilter = searchFilters.Location,
					CityFilter = searchFilters.City,
					IdentifierFilter = searchFilters.Identifier,
					StatesFilter = searchFilters.States?.Select(s=>s.ToString()),
					PickupLocations = new List<GetPickupLocationsResponse.PickupLocationDto>()
				};

				EntityQuery queryDefinition = new EntityQuery()
				{
					PageSize = 100,
					FetchState = true,
					EntityName = nameof(PickupLocationEntity),
				};

				do
				{
					EntityQueryResult queryResult = await client.ListEntitiesAsync(queryDefinition, default);

					foreach (var item in queryResult.Entities)
					{
						var locationDto = new PickupLocationDto(item.EntityId.EntityKey, (JObject)item.State);

						if (searchFilters.Match(locationDto))
							response.PickupLocations.Add(locationDto);
					}

					queryDefinition.ContinuationToken = queryResult.ContinuationToken;
				} while (queryDefinition.ContinuationToken != null);

				responseData = new OkObjectResult(response);
			}
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
                throw;
            }

            return responseData;
		}
	}

	internal class GetPickupLocationsFilters
	{
		public GetPickupLocationsFilters(HttpRequest req)
		{
			if (req.Query.ContainsKey("identifier"))
				Identifier = ((string)req.Query["identifier"]).ToLower();
			if (req.Query.ContainsKey("city"))
				City = ((string)req.Query["city"]).ToLower();
			if (req.Query.ContainsKey("location"))
				Location = ((string)req.Query["location"]).ToLower();
			if (req.Query.ContainsKey("state"))
			{
				var states = ((string)req.Query["state"]).ToLower()
					.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
				States = states.Select(s => (PickupLocationState)Enum.Parse(typeof(PickupLocationState), s, true));
			}
		}

		public bool Match(PickupLocationDto location)
		{
			bool result = true;

			if (!string.IsNullOrEmpty(Identifier))
				result &= location.Identifier.ToLower().Contains(Identifier);
			if (!string.IsNullOrEmpty(City))
				result &= location.City.ToLower().Contains(City);
			if (!string.IsNullOrEmpty(Location))
				result &= location.Location.ToLower().Contains(Location);
			if (States != null && States.Any())
				result &= States.Contains(location.CurrentStatus);

			return result;
		}

		public string Identifier { get; set; }
		public string City { get; set; }
		public string Location { get; set; }

		public IEnumerable<PickupLocationState> States { get; set; }
	}
}

