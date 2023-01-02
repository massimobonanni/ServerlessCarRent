using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Common.Dtos
{
	public class ReturnOrchestratorResponseDto
	{
		public string RentalId { get; set; }

		public string CarPlate { get; set; }

		public decimal Cost { get; set; }

		public decimal CostPerHour { get; set; }

		public string Currency { get; set; }

		public ReturnOperationState Status { get; set; }
	}

	public enum ReturnOperationState
	{ 
		Unknown,
		Complete,
		Pending,
		Error
	}
}
