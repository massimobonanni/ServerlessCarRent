using ServerlessCarRent.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessCarRent.Common.Models.CarRental;

public class CarRentalData
{
	public RentalData Rental { get; set; }
	public RenterData Renter { get; set; }
	public decimal CostPerHour { get; set; }
	public decimal TotalCost { get; set; }
	public string Currency { get; set; }
}
