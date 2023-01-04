using ServerlessCarRent.Common.Models;
using ServerlessCarRent.Common.Models.CarRental;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Common.Dtos
{
    public class RentalStatusChangeOrchestratorDto
    {
        public string CarModel { get; set; }
        public string CarPickupLocation { get; set; }
        public string CarPlate { get; set; }
        public RentalData CurrentRental { get; set; }
        public RenterData CurrentRenter { get; set; }
        public decimal CostPerHour { get; set; }
        public decimal? Cost { get; set; }
        public string Currency { get; set; }
        public CarRentalState OldRentalStatus { get; set; }
        public CarRentalState NewRentalStatus { get; set; }
    }
}
