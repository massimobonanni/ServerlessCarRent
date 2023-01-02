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
	public class RentCarResponse
	{
		[OpenApiProperty(Description = "The identifier of the rent")]
		[JsonProperty("rentId")]
		public string RentId { get; set; }

		[OpenApiProperty(Description = "The plate of the car")]
		[JsonProperty("carPlate")]
		public string CarPlate { get; set; }

		[OpenApiProperty(Description = "The identifier of the pickup location")]
		[JsonProperty("pickupLocation")]
		public string PickupLocation { get; set; }

		[OpenApiProperty(Description = "The status of the rent operation")]
		[JsonProperty("rentOperationStatus")]
		[JsonConverter(typeof(StringEnumConverter))]
		public RentOperationState RentOperationStatus { get; set; }
	}
}
