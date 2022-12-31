using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessCarRent.Common.Dtos
{
	public class RentCarPickupLocationDto
	{
		public string CarPlate { get; set; }
		public string RenterFirstName { get; set; }
		public string RenterLastName { get; set;}
		public DateTimeOffset RentalStart { get; set; }
	}
}
