using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Common.Dtos
{
	public class RentOrchestratorResponseDto
	{
		public string RentId { get; set; }
		public string CarPlate { get; set; }

		public string PickupLocation { get; set; }

		public RentOperationState Status { get; set; }
	}

	public enum RentOperationState
	{ 
		Unknown,
		Complete,
		Pending,
		Error
	}
}
