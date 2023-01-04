using AutoMapper;
using ServerlessCarRent.Functions.Requests;
using ServerlessCarRent.Functions.Responses;
using ServerlessCarRent.WebSite.Models.CarsController;
using System.Globalization;
using static ServerlessCarRent.Functions.Responses.GetCarResponse;
using static ServerlessCarRent.Functions.Responses.GetCarsResponse;

namespace ServerlessCarRent.WebSite
{
    public class MapperCarsProfile : Profile
    {
        public MapperCarsProfile()
        {
            var culture = CultureInfo.GetCultureInfo("en-US");

            CreateMap<GetCarsResponse, IndexViewModel>();
            CreateMap<CarDto, CarModel>();
            CreateMap<GetCarResponse, DetailsViewModel>()
                 .ForMember(dest => dest.CostPerHour,
                            opt => opt.MapFrom(
                                source => source.CostPerHour.ToString("#0.00", culture)));
            CreateMap<CarRentalDto, CarRentalModel>()
                .ForMember(dest=>dest.Cost,
                            opt => opt.MapFrom(
                                source => source.Cost.ToString("#0.00", culture)))
                .ForMember(dest => dest.CostPerHour,
                            opt => opt.MapFrom(
                                source => source.CostPerHour.ToString("#0.00", culture)));
            CreateMap<CreateViewModel, InitializeCarRequest>()
                .ForMember(dest => dest.CostPerHour,
                            opt => opt.MapFrom(
                                source => decimal.Parse(source.CostPerHour, culture)));
            CreateMap<GetCarResponse, EditViewModel>()
                .ForMember(dest=> dest.CostPerHour,opt=>opt.MapFrom(source=> source.CostPerHour.ToString("#.00",culture)));
            CreateMap<EditViewModel, UpdateCarRequest>()
                .ForMember(dest => dest.NewPickupLocation, opt => opt.MapFrom(source => source.PickupLocation))
                .ForMember(dest => dest.NewCostPerHour,
                            opt => opt.MapFrom<CostPerHourStringResolver, string>(source => source.CostPerHour))
                .ForMember(dest => dest.NewCarStatus, opt => opt.MapFrom(source => source.CurrentState))
                .ForMember(dest => dest.NewCurrency, opt => opt.MapFrom(source => source.Currency));
        }
    }

    public class CostPerHourStringResolver : IMemberValueResolver<object, object, string, decimal?>
    {
        public decimal? Resolve(object source, object destination,
            string sourceMember, decimal? destinationMember, ResolutionContext context)
        {
            decimal? result = null;
            if (!string.IsNullOrWhiteSpace(sourceMember))
                result = decimal.Parse(sourceMember, CultureInfo.GetCultureInfo("en-US"));
            return result;
        }

    }
}
