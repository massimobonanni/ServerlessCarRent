using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ServerlessCarRent.WebSite.Models.RentalsController
{
    public class ReturnViewModel
    {
        [DisplayName("Plate")]
        [Required()]
        public string Plate { get; set; }

        [DisplayName("Model")]
        [ValidateNever()]
        public string Model { get; set; }

        [DisplayName("Rental end date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm zzz}", ApplyFormatInEditMode = true)]
        public DateTimeOffset RentalEndDate { get; set; }

        [DisplayName("Renter first name")]
        [ValidateNever()]
        public string RenterFirstName { get; set; }

        [DisplayName("Renter last name")]
        [ValidateNever()]
        public string RenterLastName { get; set; }

        [DisplayName("Renter email")]
        [ValidateNever()]
        public string RenterEmail { get; set; }

        [ValidateNever()]
        public string ReturnAction { get; set; }
        [ValidateNever()]
        public string ReturnController { get; set; }

    }
}
