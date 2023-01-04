using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Common.Dtos
{
	public class RentOrchestratorDto
	{
		public string CarPlate { get; set; }

		public string PickupLocation { get; set; }

		public DateTimeOffset RentalStartDate { get; set; }

		public string RenterFirstName { get; set; }

		public string RenterLastName { get; set; }

        public string RenterEmail { get; set; }
    }
}
