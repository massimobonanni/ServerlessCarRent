using ServerlessCarRent.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessCarRent.Common.Interfaces
{
	public interface ICarRentalsEntity
	{
		void AddRent(CarRentalDto rentInfo);
	}
}
