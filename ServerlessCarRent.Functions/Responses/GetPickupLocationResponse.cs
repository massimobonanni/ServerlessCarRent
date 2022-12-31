using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ServerlessCarRent.Common.Models;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.CarRental;
using ServerlessCarRent.Common.Models.PickupLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Functions.Responses
{
	public class GetPickupLocationResponse
	{
		[OpenApiProperty(Description = "The identifier of the pickup location")]
		[JsonProperty("identifier")]
		public string Identifier { get; set; }

		[OpenApiProperty(Description = "The city of the pickup location")]
		[JsonProperty("city")]
		public string City { get; set; }

		[OpenApiProperty(Description = "The location of the pickup location")]
		[JsonProperty("location")]
		public string Location { get; set; }

		[OpenApiProperty(Description = "The status of the pickup location")]
		[JsonProperty("status")]
		[JsonConverter(typeof(StringEnumConverter))]
		public PickupLocationStatus Status { get; set; }

		[OpenApiProperty(Description = "The cars managed by the pickup location")]
		[JsonProperty("cars")]
		public List<PickupLocationCarDto> Cars { get; set; }

		public class PickupLocationCarDto
		{
			[OpenApiProperty(Description = "The plate of the car")]
			[JsonProperty("plate")] 
			public string Plate { get; set; }

			[OpenApiProperty(Description = "The model of the car")]
			[JsonProperty("model")]
			public string Model { get; set; }

			[OpenApiProperty(Description = "The status of the car")]
			[JsonProperty("status")]
			[JsonConverter(typeof(StringEnumConverter))] 
			public CarState Status { get; set; }

			[OpenApiProperty(Description = "The status of the car rental")]
			[JsonProperty("rentalStatus")]
			[JsonConverter(typeof(StringEnumConverter))] 
			public CarRentalState RentalStatus { get; set; }
		}
	}

	
}
