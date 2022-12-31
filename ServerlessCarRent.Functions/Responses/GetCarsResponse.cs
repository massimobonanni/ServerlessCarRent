using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using ServerlessCarRent.Common.Models;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.CarRental;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ServerlessCarRent.Functions.Responses.GetCarsResponse;

namespace ServerlessCarRent.Functions.Responses
{
	public class GetCarsResponse
	{
		[OpenApiProperty(Description = "The search parameter for the plate")]
		[JsonProperty("plateFilter")]
		public string PlateFilter { get; set; }

		[OpenApiProperty(Description = "The search parameter for the model")]
		[JsonProperty("modelFilter")]
		public string ModelFilter { get; set; }

		[OpenApiProperty(Description = "The search parameter for the location")]
		[JsonProperty("locationFilter")]
		public string LocationFilter { get; set; }

		[OpenApiProperty(Description = "The search parameter for the current states")]
		[JsonProperty("statesFilter")]
		[JsonConverter(typeof(StringEnumConverter))]
		public List<CarState> StatesFilter { get; set; }

		[OpenApiProperty(Description = "The search parameter for the current rental states of the car")]
		[JsonProperty("rentalStatesFilter")]
		[JsonConverter(typeof(StringEnumConverter))]
		public List<CarRentalState> RentalStatesFilter { get; set; }


		[OpenApiProperty(Description = "The search results")]
		[JsonProperty("cars")]
		public List<CarDto> Cars { get; set; }

		public class CarDto
		{
			public CarDto()
			{

			}

			public CarDto(string plate, CarData carData)
			{
				CurrentRentalState = carData.CurrentRentalState;
				CurrentState = carData.CurrentState;
				Model = carData.Model;
				PickupLocation = carData.PickupLocation;
				Plate = plate;
			}

			public CarDto(string plate,JObject entityState )
			{
				var carStatusProperty = (JObject)entityState.Property("Status").Value;
				var carData = carStatusProperty.ToObject<CarData>();
				CurrentRentalState = carData.CurrentRentalState;
				CurrentState = carData.CurrentState;
				Model = carData.Model;
				PickupLocation = carData.PickupLocation;
				Plate = plate;
			}

			[OpenApiProperty(Description = "The plate of the car")]
			[JsonProperty("plate")]
			public string Plate { get; set; }

			[OpenApiProperty(Description = "The model of the car")]
			[JsonProperty("model")]
			public string Model { get; set; }

			[OpenApiProperty(Description = "The location where the car can be rented")]
			[JsonProperty("pickupLocation")]
			public string PickupLocation { get; set; }

			[OpenApiProperty(Description = "The current state of the car")]
			[JsonProperty("currentState")]
			[JsonConverter(typeof(StringEnumConverter))]
			public CarState CurrentState { get; set; }

			[OpenApiProperty(Description = "The current rental state of the car")]
			[JsonProperty("currentRentalState")]
			[JsonConverter(typeof(StringEnumConverter))]
			public CarRentalState CurrentRentalState { get; set; }
		}
	}


}
