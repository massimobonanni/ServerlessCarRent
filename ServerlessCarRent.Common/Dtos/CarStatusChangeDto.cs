using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.CarRental;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Common.Dtos
{
	public class CarStatusChangeDto
	{
		public string CarPlate { get; set; }

		public string NewCarModel { get; set; }

		public string NewPickupLocation { get; set; }

		public CarState? NewCarStatus { get; set; }

		public CarRentalState? NewCarRentalStatus { get; set; }

	}
}
