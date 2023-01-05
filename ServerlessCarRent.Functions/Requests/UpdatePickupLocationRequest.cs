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
	public class UpdatePickupLocationRequest
	{

		[OpenApiProperty(Description = "The pickup location city.")]
		[JsonProperty("city")]
		public string City { get; set; }

		[OpenApiProperty(Description = "The pickup location description.")]
		[JsonProperty("location")]
		public string Location { get; set; }

		[OpenApiProperty(Description = "The pickup location status")]
		[JsonProperty("status")]
		[JsonConverter(typeof(StringEnumConverter))]
		public PickupLocationState Status { get; set; }

		internal bool IsValid()
		{
			bool retVal = true;
			retVal &= !string.IsNullOrWhiteSpace(City);
			retVal &= !string.IsNullOrWhiteSpace(Location);
			return retVal;
		}
	}
}
