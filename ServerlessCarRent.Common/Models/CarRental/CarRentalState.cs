using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Common.Models.CarRental
{
	public enum CarRentalState
	{
		Unknown,
		Free,
		Booked,
		Rented
	}
}
