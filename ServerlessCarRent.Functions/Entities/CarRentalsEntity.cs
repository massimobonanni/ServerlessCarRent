using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ServerlessCarRent.Common.Dtos;
using ServerlessCarRent.Common.Interfaces;
using ServerlessCarRent.Common.Models;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.CarRental;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ServerlessCarRent.Functions.Entities
{
	public class CarRentalsEntity : ICarRentalsEntity
	{
		private readonly ILogger _logger;
		public CarRentalsEntity(ILogger logger)
		{
			_logger = logger;
		}

		[JsonPropertyName("status")]
		public CarRentalsData Status { get; set; }

		public void AddRent(CarRentalDto rentInfo)
		{
			if (this.Status == null)
				this.Status = new CarRentalsData();

			if (this.Status.Rentals == null)
				this.Status.Rentals = new List<CarRentalData>();

			this.Status.Rentals.Add(new CarRentalData()
			{
				CostPerHour = rentInfo.CostPerHour,
				Currency = rentInfo.Currency,
				Rental = rentInfo.Rental,
				Renter = rentInfo.Renter
			});
		}

		[FunctionName(nameof(CarRentalsEntity))]
		public static Task Run([EntityTrigger] IDurableEntityContext ctx, ILogger logger)
			=> ctx.DispatchAsync<CarRentalsEntity>(logger);
	}
}
