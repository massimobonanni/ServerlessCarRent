using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ServerlessCarRent.Common.Models.PickupLocation;

namespace ServerlessCarRent.Functions.Responses
{
    public class GetPickupLocationsResponse
	{
		[OpenApiProperty(Description = "The search parameter for the identifier")]
		[JsonProperty("identifierFilter")]
		public string? IdentifierFilter { get; set; }

		[OpenApiProperty(Description = "The search parameter for the city")]
		[JsonProperty("cityFilter")]
		public string? CityFilter { get; set; }

		[OpenApiProperty(Description = "The search parameter for the location")]
		[JsonProperty("locationFilter")]
		public string? LocationFilter { get; set; }

		[OpenApiProperty(Description = "The search parameter for the current states")]
		[JsonProperty("statesFilter")]
		public IEnumerable<string>? StatesFilter { get; set; }


		[OpenApiProperty(Description = "The search results")]
		[JsonProperty("pickupLocations")]
		public List<PickupLocationDto>? PickupLocations { get; set; }

		public class PickupLocationDto
		{
			public PickupLocationDto()
			{

			}

			public PickupLocationDto(string identifier, PickupLocationData locationData)
			{
				CurrentStatus = locationData.Status;
				City=locationData.City;
				Location=locationData.Location;
				Identifier=identifier;
			}

            public PickupLocationDto(string identifier, string serializedEntityState)
            {
                var locationData = JsonConvert.DeserializeObject<PickupLocationData>(serializedEntityState);
                CurrentStatus = locationData!.Status;
                City = locationData.City;
                Location = locationData.Location;
                Identifier = identifier;
            }

			[OpenApiProperty(Description = "The identifier of the pickup location")]
			[JsonProperty("identifier")]
			public string Identifier { get; set; }

			[OpenApiProperty(Description = "The city of the pickup location")]
			[JsonProperty("city")]
			public string City { get; set; }

			[OpenApiProperty(Description = "The description for the pickup location")]
			[JsonProperty("location")]
			public string Location { get; set; }

			[OpenApiProperty(Description = "The current state of the location")]
			[JsonProperty("currentState")]
			[JsonConverter(typeof(StringEnumConverter))]
			public PickupLocationState CurrentStatus { get; set; }

		}
	}


}
