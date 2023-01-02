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
	public class UpdateCarRequest
	{
		[OpenApiProperty(Description = "The new pickup location. Leave empty if you don't want to change.")]
		[JsonProperty("newPickupLocation")]
		public string NewPickupLocation { get; set; }

		[OpenApiProperty(Description = "The new car status. Leave empty if you don't want to change.")]
		[JsonProperty("newCarStatus")]
		[JsonConverter(typeof(StringEnumConverter))]
		public CarState? NewCarStatus { get; set; }

		[OpenApiProperty(Description = "The new cost per hour. Leave empty if you don't want to change.")]
		[JsonProperty("newCostPerHour")]
		public decimal? NewCostPerHour { get; set; }

		[OpenApiProperty(Description = "The new currency. Leave empty if you don't want to change.")]
		[JsonProperty("newCurrency")]
		public string NewCurrency { get; set; }

	}
}
