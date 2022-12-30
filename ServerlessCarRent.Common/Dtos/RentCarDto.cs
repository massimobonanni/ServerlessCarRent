using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessCarRent.Common.Dtos
{
	public class RentCarDto
	{
		public DateTimeOffset StartDate { get; set; }
		public string RentalFirstName { get; set; }
		public string RentalLastName { get; set; }
	}
}
