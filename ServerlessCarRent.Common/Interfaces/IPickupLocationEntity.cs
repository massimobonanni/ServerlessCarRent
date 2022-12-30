using ServerlessCarRent.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessCarRent.Common.Interfaces
{
	public interface IPickupLocationEntity
	{
		void Initialize(InitializePickupLocationDto locationInfo);

		void RentCar(RentCarPickupLocationDto carInfo);

		void CarStatusChanged(CarStatusChangeDto carInfo);
	}
}
