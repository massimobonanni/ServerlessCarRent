using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.CarRental;
using System.ComponentModel;

namespace ServerlessCarRent.WebSite.Models.CarsController
{
    public class CarModel
    {
        public string Plate { get; set; }

        public string Model { get; set; }

        [DisplayName("Pickup location")]
        public string PickupLocation { get; set; }

        [DisplayName("Car state")]
        public CarState CurrentStatus { get; set; }

        [DisplayName("Rental state")]
        public CarRentalState CurrentRentalStatus { get; set; }
    }
}
