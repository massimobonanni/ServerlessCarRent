using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Common.Dtos
{
	public class ReturnOrchestratorDto
	{
		public string CarPlate { get; set; }

		public DateTimeOffset RentalEndDate { get; set; }

	}
}
