using ServerlessCarRent.Common.Models.Car;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessCarRent.Common.Dtos
{
	public class InitializeCarDto
	{
		public string Model { get; set; }
		public string PickupLocation { get; set; }
		public CarState CarStatus { get; set; } = CarState.Working;

		public decimal CostPerHour { get; set; }
		public string Currency { get; set; }
	}
}
