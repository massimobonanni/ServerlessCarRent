using AutoMapper;
using ServerlessCarRent.Functions.Requests;
using ServerlessCarRent.Functions.Responses;
using ServerlessCarRent.WebSite.Models.RentalsController;
using System.Globalization;

namespace ServerlessCarRent.WebSite
{
    public class MapperRentalsProfile : Profile
    {
        public MapperRentalsProfile()
        {
            var culture = CultureInfo.GetCultureInfo("en-US");

            CreateMap<GetCarResponse, RentViewModel>()
                .ForMember(dest => dest.CarPlate, opt => opt.MapFrom(source => source.Plate))
                .ForMember(dest => dest.CarModel, opt => opt.MapFrom(source => source.Model));
            CreateMap<GetPickupLocationResponse, RentViewModel>()
                .ForMember(dest => dest.PickupLocation, opt => opt.MapFrom(source => source.Identifier))
                .ForMember(dest => dest.PickupLocationDescription, opt => opt.MapFrom(source => source.Location));
            CreateMap<RentViewModel, RentCarRequest>();
            CreateMap<GetCarResponse, ReturnViewModel>()
                .ForMember(dest => dest.RenterLastName, opt => opt.MapFrom(source => source.CurrentRenterLastName))
                .ForMember(dest => dest.RenterFirstName, opt => opt.MapFrom(source => source.CurrentRenterFirstName))
                .ForMember(dest => dest.RenterEmail, opt => opt.MapFrom(source => source.CurrentRenterEmail));
            CreateMap<ReturnViewModel, ReturnCarRequest>();
        }
    }


}
