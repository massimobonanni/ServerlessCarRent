using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.CarRental;
using System.ComponentModel;
using YamlDotNet.Core.Tokens;

namespace ServerlessCarRent.WebSite.Models.CarsController
{
    public class IndexViewModel
    {
        [DisplayName("Plate")]
        public string PlateFilter { get; set; }

        [DisplayName("Model")]
        public string ModelFilter { get; set; }

        [DisplayName("Pickup location")]
        public string LocationFilter { get; set; }

        public bool CarWorking { get; set; }

        public bool CarUnusable { get; set; }

        public bool CarUnderMaintenance { get; set; }

        public IEnumerable<CarState> StatesFilter
        {
            get
            {
                var result = new List<CarState>();
                if (this.CarWorking)
                    result.Add(CarState.Working);
                if (this.CarUnusable)
                    result.Add(CarState.Unusable);
                if (this.CarUnderMaintenance)
                    result.Add(CarState.UnderMaintenance);
                return result;
            }
            set
            {
                this.CarUnderMaintenance = value.Contains(CarState.UnderMaintenance);
                this.CarUnusable = value.Contains(CarState.Unusable);
                this.CarWorking = value.Contains(CarState.Working);
            }
        }

        public bool CarRentFree { get; set; }

        public bool CarRentRented { get; set; }

        public IEnumerable<CarRentalState> RentalStatesFilter
        {
            get
            {
                var result = new List<CarRentalState>();
                if (this.CarRentFree)
                    result.Add(CarRentalState.Free);
                if (this.CarRentRented)
                    result.Add(CarRentalState.Rented);
                return result;
            }
            set
            {
                this.CarRentFree = value.Contains(CarRentalState.Free);
                this.CarRentRented = value.Contains(CarRentalState.Rented);
            }
        }

        public List<CarModel> Cars { get; set; }

    }
}
