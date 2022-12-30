using ServerlessCarRent.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessCarRent.Common.Interfaces
{
	public interface ICarEntity
	{
		void Initialize(InitializeCarDto carInfo);
		void Rent(RentCarDto rentInfo);
		void Return(ReturnCarDto returnInfo);

		void Update(UpdateCarDto info);
	}
}
