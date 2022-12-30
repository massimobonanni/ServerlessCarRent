using ServerlessCarRent.Common.Models.Car;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Common.Dtos
{
	public class UpdateCarDto
	{
		public string NewPickupLocation { get; set; }

		public CarState? NewCarStatus { get; set; }

		public decimal? NewCostPerHour { get; set; }

		public string NewCurrency { get; set; }
	}
}
