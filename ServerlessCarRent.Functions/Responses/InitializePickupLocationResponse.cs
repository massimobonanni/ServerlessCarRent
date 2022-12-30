using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Functions.Responses
{
	public class InitializePickupLocationResponse
	{
		[OpenApiProperty(Description = "The identifier of the pickup location")]
		[JsonProperty("identifier")]
		public string Identifier { get; set; }
	}
}
