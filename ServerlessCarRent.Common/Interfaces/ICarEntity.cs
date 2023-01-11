using ServerlessCarRent.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Common.Interfaces
{
	public interface ICarEntity
	{
		void Initialize(InitializeCarDto carInfo);
		void Rent(RentCarDto rentInfo);
		Task<ReturnCarResponseDto> Return(ReturnCarDto returnInfo);
		void Update(UpdateCarDto info);
	}
}
