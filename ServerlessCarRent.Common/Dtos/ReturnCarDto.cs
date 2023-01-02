using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessCarRent.Common.Dtos
{
	public class ReturnCarDto
	{
		public string RentalId { get; set; }
		public DateTimeOffset EndDate { get; set; }
	}
}
