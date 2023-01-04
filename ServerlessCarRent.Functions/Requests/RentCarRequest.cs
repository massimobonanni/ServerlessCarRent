using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServerlessCarRent.Functions.Requests
{
	public class RentCarRequest
	{
		[OpenApiProperty(Description = "The plate of the car")]
		[JsonProperty("carPlate")]
		public string CarPlate { get; set; }

		[OpenApiProperty(Description = "The identifier of the pickup location")]
		[JsonProperty("pickupLocation")]
		public string PickupLocation { get; set; }

		[OpenApiProperty(Description = "The rental start date")]
		[JsonProperty("rentalStartDate")] 
		public DateTimeOffset RentalStartDate { get; set; }

		[OpenApiProperty(Description = "The renter first name")]
		[JsonProperty("renterFirstName")] 
		public string RenterFirstName { get; set; }

		[OpenApiProperty(Description = "The renter last name")]
		[JsonProperty("renterLastName")]
		public string RenterLastName { get; set; }

        [OpenApiProperty(Description = "The renter email")]
        [JsonProperty("renterEmail")]
        public string RenterEmail { get; set; }

        internal bool IsValid()
		{
			bool retVal = true;
			retVal &= !string.IsNullOrWhiteSpace(CarPlate);
			retVal &= !string.IsNullOrWhiteSpace(PickupLocation);
			retVal &= !string.IsNullOrWhiteSpace(RenterFirstName);
			retVal &= !string.IsNullOrWhiteSpace(RenterLastName);
            retVal &= !string.IsNullOrWhiteSpace(RenterEmail);

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(RenterEmail);
            retVal &= match.Success;

            return retVal;
		}
	}
}
