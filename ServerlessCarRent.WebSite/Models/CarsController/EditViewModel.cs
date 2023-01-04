using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.CarRental;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ServerlessCarRent.WebSite.Models.CarsController
{
    public class EditViewModel
    {
        [ValidateNever()]
        public string Plate { get; set; }

        [ValidateNever()]
        public string Model { get; set; }

        [DisplayName("Pickup location")]
        public string? PickupLocation { get; set; }

        [DisplayName("Car state")]
        public CarState CurrentState { get; set; }

        [RegularExpression(@"^(?:|\d{1,2}(?:\.\d{0,6})?)$", ErrorMessage = "Valid Decimal number with maximum 2 decimal places.")]
        public string? CostPerHour { get; set; }

        [Required()]
        public string Currency { get; set; }

        [ValidateNever()]
        public List<SelectListItem> Currencies { get; set; }

        [ValidateNever()]
        public List<SelectListItem> CarStates { get; set; }
    }
}
