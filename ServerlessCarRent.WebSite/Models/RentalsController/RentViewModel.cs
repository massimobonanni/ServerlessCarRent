using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ServerlessCarRent.WebSite.Models.RentalsController
{
    public class RentViewModel
    {
        [DisplayName("Plate")]
        [Required()]
        public string CarPlate { get; set; }

        [DisplayName("Model")]
        public string CarModel { get; set; }

        [DisplayName("Pickup Location")]
        [Required()]
        public string PickupLocation { get; set; }

        [DisplayName("Pickup Location description")]
        public string PickupLocationDescription { get; set; }

        [DisplayName("Rental start date")]
        [DisplayFormat(DataFormatString ="{0:dd/MM/yyyy HH:mm zzz}", ApplyFormatInEditMode = true)]
        public DateTimeOffset RentalStartDate { get; set; }

        [DisplayName("Renter first name")]
        [Required()]
        public string RenterFirstName { get; set; }

        [DisplayName("Renter last name")]
        [Required()]
        public string RenterLastName { get; set; }

        [DisplayName("Renter email")]
        [Required()]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string RenterEmail { get; set; }

        [ValidateNever()]
        public string ReturnAction { get; set; }
        [ValidateNever()]
        public string ReturnController { get; set; }
    }
}
