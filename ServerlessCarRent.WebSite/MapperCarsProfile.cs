using AutoMapper;
using ServerlessCarRent.Functions.Responses;
using ServerlessCarRent.WebSite.Models.CarsController;
using static ServerlessCarRent.Functions.Responses.GetCarResponse;
using static ServerlessCarRent.Functions.Responses.GetCarsResponse;

namespace ServerlessCarRent.WebSite
{
	public  class MapperCarsProfile:Profile
	{
		public MapperCarsProfile()
		{
			CreateMap<GetCarsResponse, IndexViewModel>();
			CreateMap<CarDto, CarModel>();
			CreateMap<GetCarResponse, DetailsViewModel>();
			CreateMap<CarRentalDto,CarRentalModel>();
		}
	}
}
