using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using ServerlessCarRent.Common.Models.Car;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ServerlessCarRent.WebSite.Models.CarsController
{
    public class CreateViewModel
    {
        [Required()]
        public string Plate { get; set; }

        [Required()]
        public string Model { get; set; }

        [Required()]
        [DisplayName("Pickup location")]
        public string PickupLocation { get; set; }

        [Required()]
        [RegularExpression(@"^[0-9]+(\.[0-9]{1,2})$", ErrorMessage = "Valid Decimal number with maximum 2 decimal places.")]
        public string CostPerHour { get; set; }

        [Required()]
        public string Currency { get; set; }

        [ValidateNever()]
        public List<SelectListItem> Currencies { get; set; }

        [ValidateNever()]
        public List<SelectListItem> PickupLocations { get; set; }

    }
}
