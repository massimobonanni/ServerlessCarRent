using ServerlessCarRent.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessCarRent.Common.Dtos
{
	public class CarRentalDto
	{
		public RentalData Rental { get; set; }
		public RenterData Renter { get; set; }
		public decimal CostPerHour { get; set; }
		public decimal Cost { get; set; }
		public string Currency { get; set; }
	}
}
