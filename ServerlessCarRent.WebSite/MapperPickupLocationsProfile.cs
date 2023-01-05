using AutoMapper;
using ServerlessCarRent.Functions.Requests;
using ServerlessCarRent.Functions.Responses;
using ServerlessCarRent.WebSite.Models.PickupLocationsController;
using System.Globalization;
using static ServerlessCarRent.Functions.Responses.GetPickupLocationResponse;
using static ServerlessCarRent.Functions.Responses.GetPickupLocationsResponse;

namespace ServerlessCarRent.WebSite
{
    public class MapperPickupLocationsProfile : Profile
    {
        public MapperPickupLocationsProfile()
        {
            var culture = CultureInfo.GetCultureInfo("en-US");

            CreateMap<GetPickupLocationsResponse, IndexViewModel>();
            CreateMap<PickupLocationDto, PickupLocationModel>();
            CreateMap<GetPickupLocationResponse, DetailsViewModel>();
            CreateMap<PickupLocationCarDto, PickupLocationCarModel>();
            CreateMap<CreateViewModel, InitializePickupLocationRequest>();
        }
    }


}
