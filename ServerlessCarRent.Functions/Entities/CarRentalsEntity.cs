using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ServerlessCarRent.Common.Dtos;
using ServerlessCarRent.Common.Interfaces;
using ServerlessCarRent.Common.Models.CarRental;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.DurableTask.Entities;
using Microsoft.Azure.Functions.Worker;
using EntityTriggerAttribute = Microsoft.Azure.Functions.Worker.EntityTriggerAttribute;

namespace ServerlessCarRent.Functions.Entities
{
    public class CarRentalsEntity : TaskEntity<CarRentalsData>,ICarRentalsEntity

    {
		private readonly ILogger _logger;
		public CarRentalsEntity(ILogger logger)
		{
			_logger = logger;
		}

		public void AddRent(CarRentalDto rentInfo)
		{
			if (this.State == null)
				this.State = new CarRentalsData();

			if (this.State.Rentals == null)
				this.State.Rentals = new List<CarRentalData>();

			this.State.Rentals.Add(new CarRentalData()
			{
				TotalCost=rentInfo.Cost,
				CostPerHour = rentInfo.CostPerHour,
				Currency = rentInfo.Currency,
				Rental = rentInfo.Rental,
				Renter = rentInfo.Renter
			});
		}

		[FunctionName(nameof(CarRentalsEntity))]
		public static Task Run([EntityTrigger] TaskEntityDispatcher ctx)
			=> ctx.DispatchAsync<CarRentalsEntity>();
	}
}
