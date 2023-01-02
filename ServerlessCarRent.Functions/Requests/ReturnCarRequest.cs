using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Functions.Requests
{
	public class ReturnCarRequest
	{
		[OpenApiProperty(Description = "The rental end date")]
		[JsonProperty("rentalEndDate")] 
		public DateTimeOffset RentalEndDate { get; set; }

	}
}
