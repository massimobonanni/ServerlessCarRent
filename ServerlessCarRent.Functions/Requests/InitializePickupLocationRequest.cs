using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.PickupLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServerlessCarRent.Functions.Requests
{
	public class InitializePickupLocationRequest
	{
		[OpenApiProperty(Description = "The pickup location identifier. this is the identifier of the location used in the cars.")]
		[JsonProperty("identifier")] 
		public string Identifier { get; set; }

		[OpenApiProperty(Description = "The pickup location city.")]
		[JsonProperty("city")]
		public string City { get; set; }

		[OpenApiProperty(Description = "The pickup location description.")]
		[JsonProperty("location")]
		public string Location { get; set; }

		[OpenApiProperty(Description = "The pickup location status")]
		[JsonProperty("status")]
		[JsonConverter(typeof(StringEnumConverter))]
		public PickupLocationStatus Status { get; set; }

		internal bool IsValid()
		{
			bool retVal = true;
			retVal &= !string.IsNullOrWhiteSpace(Identifier);
			retVal &= !string.IsNullOrWhiteSpace(City);
			retVal &= !string.IsNullOrWhiteSpace(Location);
			return retVal;
		}
	}
}
