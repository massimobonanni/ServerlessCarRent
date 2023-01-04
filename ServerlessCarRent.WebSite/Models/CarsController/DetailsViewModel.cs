using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.CarRental;
using System.ComponentModel.DataAnnotations;
using DisplayAttribute = System.ComponentModel.DataAnnotations.DisplayAttribute;
using System.ComponentModel;

namespace ServerlessCarRent.WebSite.Models.CarsController
{
    public class DetailsViewModel
    {
         public string Plate { get; set; }

        public string Model { get; set; }

        [DisplayName("Pickup location")]
        public string PickupLocation { get; set; }

        [DisplayName("Car state")]
        public CarState CurrentState { get; set; }

        [DisplayName("Rental state")]
        public CarRentalState CurrentRentalState { get; set; }

        [DisplayName("Cost per hour")]
        [DisplayFormat(DataFormatString = "{0:##,###0.00}")]
        public decimal CostPerHour { get; set; }

        public string Currency { get; set; }

        [DisplayName("Renter first name")]
        public string CurrentRenterFirstName { get; set; }

        [DisplayName("Renter last name")]
        public string CurrentRenterLastName { get; set; }
        
        [DisplayName("Renter email")]
        public string CurrentRenterEmail { get; set; }

        public List<CarRentalModel> Rentals { get; set; }


    }
}
