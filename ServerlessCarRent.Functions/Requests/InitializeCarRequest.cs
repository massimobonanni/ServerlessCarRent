using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ServerlessCarRent.Common.Models.Car;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServerlessCarRent.Functions.Requests
{
	public class InitializeCarRequest
	{
		[OpenApiProperty(Description = "The plate of the car")]
		[JsonProperty("plate")]
		public string Plate { get; set; }

		[OpenApiProperty(Description = "The model of the car")]
		[JsonProperty("model")]
		public string Model { get; set; }

		[OpenApiProperty(Description = "The pickup location of the car. Can be null.")]
		[JsonProperty("pickupLocation")]
		public string PickupLocation { get; set; }

		[OpenApiProperty(Description = "The current status for the car")]
		[JsonProperty("currentStatus")]
		[JsonConverter(typeof(StringEnumConverter))]
		public CarState CurrentStatus { get; set; } = CarState.Working;

		[OpenApiProperty(Description = "The cost for each hour of rent")]
		[JsonProperty("costPerHour")]
		public decimal CostPerHour { get; set; }

		[OpenApiProperty(Description = "The currency for the cost")]
		[JsonProperty("currency")] 
		public string Currency { get; set; }
		
		internal bool IsValid()
		{
			bool retVal = true;
			retVal &= !string.IsNullOrWhiteSpace(Plate);
			retVal &= !string.IsNullOrWhiteSpace(Model);
			return retVal;
		}
	}
}
