using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ServerlessCarRent.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Functions.Responses
{
	public class ReturnCarResponse
	{
		[OpenApiProperty(Description = "The identifier of the rent")]
		[JsonProperty("rentalId")]
		public string RentalId { get; set; }

		[OpenApiProperty(Description = "The plate of the car")]
		[JsonProperty("carPlate")]
		public string CarPlate { get; set; }

		[OpenApiProperty(Description = "The cost of the rent")]
		[JsonProperty("cost")]
		public decimal Cost { get; set; }

		[OpenApiProperty(Description = "The cost of the rent per hour")]
		[JsonProperty("costPerHour")]
		public decimal CostPerHour { get; set; }

		[OpenApiProperty(Description = "The currency")]
		[JsonProperty("currency")]
		public string Currency { get; set; }

		[OpenApiProperty(Description = "The status of the return operation")]
		[JsonProperty("returnOperationStatus")]
		[JsonConverter(typeof(StringEnumConverter))]
		public ReturnOperationState ReturnOperationStatus { get; set; }
	}
}
