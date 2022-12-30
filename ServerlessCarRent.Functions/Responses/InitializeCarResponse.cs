using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Functions.Responses
{
	public class InitializeCarResponse
	{
		[OpenApiProperty(Description = "The plate of the car")]
		[JsonProperty("plate")]
		public string Plate { get; set; }
	}
}
