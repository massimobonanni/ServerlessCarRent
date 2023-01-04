using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ServerlessCarRent.WebSite.Models.CarsController
{
    public class CarRentalModel
    {
        [DisplayName("Rental Id")]
        public string RentalId { get; set; }

        [DisplayName("Rental start")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm zzz}")]
        public DateTimeOffset RentalStart { get; set; }

        [DisplayName("Rental end")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm zzz}")]
        public DateTimeOffset? RentalEnd { get; set; }

        [DisplayName("Renter first name")]
        public string RenterFirstName { get; set; }

        [DisplayName("Renter last name")]
        public string RenterLastName { get; set; }

        [DisplayFormat(DataFormatString = "{0:##,###0.00}")]
        [DisplayName("Cost per hour")] 
        public decimal CostPerHour { get; set; }

        [DisplayFormat(DataFormatString = "{0:##,###0.00}")]
        [DisplayName("Total cost")]
        public decimal Cost { get; set; }

        public string Currency { get; set; }
    }
}
