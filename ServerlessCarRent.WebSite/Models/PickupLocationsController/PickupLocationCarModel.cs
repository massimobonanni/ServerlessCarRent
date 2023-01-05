using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.CarRental;
using System.ComponentModel;

namespace ServerlessCarRent.WebSite.Models.PickupLocationsController
{
    public class PickupLocationCarModel
    {
        [DisplayName("Plate")]
        public string Plate { get; set; }

        [DisplayName("Model")]
        public string Model { get; set; }

        [DisplayName("Car Status")]
        public CarState Status { get; set; }

        [DisplayName("Rental Status")]
        public CarRentalState RentalStatus { get; set; }
    }
}
