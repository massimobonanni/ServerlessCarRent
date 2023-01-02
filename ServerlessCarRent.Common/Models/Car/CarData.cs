using ServerlessCarRent.Common.Models.CarRental;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Common.Models.Car
{
	public class CarData
	{
		public string Model { get; set; }
		public string PickupLocation { get; set; }
		public CarState CurrentState { get; set; }
		public CarRentalState CurrentRentalState { get; set; }
		public RenterData CurrentRenter { get; set; }
		public RentalData CurrentRental { get; set; }

		public decimal CostPerHour { get; set; }
		public string Currency { get; set; }

		public bool CanBeRent()
		{
			var rentStatus= this.CurrentRentalState != Common.Models.CarRental.CarRentalState.Rented
				&& this.CurrentRentalState != Common.Models.CarRental.CarRentalState.Unknown;

			var carStatus =this.CurrentState==CarState.Working;

			return rentStatus & carStatus;
		}
		public bool CanBeReturn()
		{
			return this.CurrentRentalState == Common.Models.CarRental.CarRentalState.Rented;
		}
	}
}
