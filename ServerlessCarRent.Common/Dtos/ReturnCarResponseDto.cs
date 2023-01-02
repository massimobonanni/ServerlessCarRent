using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessCarRent.Common.Dtos
{
	public class ReturnCarResponseDto
	{
		public string RentalId { get; set; }

		public decimal Cost { get; set; }

		public decimal CostPerHour { get; set; }

		public string Currency { get; set; }
		public bool Succeeded { get; set; }
	}

}
