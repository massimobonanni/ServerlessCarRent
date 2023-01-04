using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessCarRent.Common.Dtos
{
	public class RentCarDto
	{
		public string RentalId { get; set; }
		public DateTimeOffset StartDate { get; set; }
		public string RenterFirstName { get; set; }
		public string RenterLastName { get; set; }
        public string RenterEmail { get; set; }
    }
}
