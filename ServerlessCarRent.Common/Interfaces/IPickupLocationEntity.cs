using ServerlessCarRent.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Common.Interfaces
{
	public interface IPickupLocationEntity
	{
		void Initialize(InitializePickupLocationDto locationInfo);

		Task<bool> RentCar(RentCarPickupLocationDto carInfo);

		void CarStatusChanged(CarStatusChangeDto carInfo);
	}
}
