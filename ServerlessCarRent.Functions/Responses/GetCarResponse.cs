using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ServerlessCarRent.Common.Models;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.CarRental;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Functions.Responses
{
	public class GetCarResponse
	{
		[OpenApiProperty(Description = "The plate of the car")]
		[JsonProperty("plate")]
		public string? Plate { get; set; }

		[OpenApiProperty(Description = "The model of the car")]
		[JsonProperty("model")]
		public string? Model { get; set; }

		[OpenApiProperty(Description = "The location where the car can be rented")]
		[JsonProperty("pickupLocation")]
		public string? PickupLocation { get; set; }

		[OpenApiProperty(Description = "The current state of the car")]
		[JsonProperty("currentState")]
		[JsonConverter(typeof(StringEnumConverter))]
		public CarState CurrentState { get; set; }

		[OpenApiProperty(Description = "The current rental state of the car")]
		[JsonProperty("currentRentalState")]
		[JsonConverter(typeof(StringEnumConverter))]
		public CarRentalState CurrentRentalState { get; set; }

        [OpenApiProperty(Description = "Current renter first name")]
        [JsonProperty("currentRenterFirstName")]
        public string? CurrentRenterFirstName { get; set; }

        [OpenApiProperty(Description = "Current renter email address")]
        [JsonProperty("currentRenterEmail")]
        public string? CurrentRenterEmail { get; set; }

        [OpenApiProperty(Description = "Current renter last name")]
        [JsonProperty("currentRenterLastName")]
        public string? CurrentRenterLastName { get; set; }


        [OpenApiProperty(Description = "The cost for each hour of rent")]
		[JsonProperty("costPerHour")]
		public decimal CostPerHour { get; set; }

		[OpenApiProperty(Description = "The currency for the cost")]
		[JsonProperty("currency")]
		public string? Currency { get; set; }

		[OpenApiProperty(Description = "History of rentals for the car")]
		[JsonProperty("rentals")]
		public List<CarRentalDto>? Rentals { get; set; }

		public class CarRentalDto
		{

			[OpenApiProperty(Description = "Rental id")]
			[JsonProperty("rentalId")]
			public string? RentalId { get; set; }

			[OpenApiProperty(Description = "Rental start date")]
			[JsonProperty("rentalStart")]
			public DateTimeOffset RentalStart { get; set; }

			[OpenApiProperty(Description = "Rental end date")]
			[JsonProperty("rentalEnd")]
			public DateTimeOffset? RentalEnd { get; set; }

			[OpenApiProperty(Description = "Renter first name")]
			[JsonProperty("renterFirstName")]
			public string? RenterFirstName { get; set; }

            [OpenApiProperty(Description = "Renter email address")]
            [JsonProperty("renterEmail")]
            public string? RenterEmail { get; set; }

            [OpenApiProperty(Description = "Renter last name")]
			[JsonProperty("renterLastName")]
			public string? RenterLastName { get; set; }

			[OpenApiProperty(Description = "Cost of the rent per hour")]
			[JsonProperty("costPerHour")]
			public decimal CostPerHour { get; set; }

			[OpenApiProperty(Description = "Total cost of the rent")]
			[JsonProperty("cost")]
			public decimal Cost { get; set; }

			[OpenApiProperty(Description = "The currency of the rental cost")]
			[JsonProperty("currency")]
			public string? Currency { get; set; }
		}
	}

	
}
