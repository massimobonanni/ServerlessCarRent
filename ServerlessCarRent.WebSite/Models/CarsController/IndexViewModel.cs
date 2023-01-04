using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.CarRental;
using System.ComponentModel;

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

        [DisplayName("Car states")]
        public IEnumerable<string> StatesFilter { get; set; }

        [DisplayName("Rental states")]
        public IEnumerable<string> RentalStatesFilter { get; set; }

        public List<CarModel> Cars { get; set; }

    }
}
