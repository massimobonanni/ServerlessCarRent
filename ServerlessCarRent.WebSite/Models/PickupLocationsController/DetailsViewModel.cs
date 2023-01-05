using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.CarRental;
using ServerlessCarRent.Common.Models.PickupLocation;
using System.ComponentModel;

namespace ServerlessCarRent.WebSite.Models.PickupLocationsController
{
    public class DetailsViewModel
    {
        [DisplayName("Location Id")]
        public string Identifier { get; set; }

        [DisplayName("City")]
        public string City { get; set; }

        [DisplayName("Description")]
        public string Location { get; set; }

        [DisplayName("Location Status")]
        public PickupLocationState Status { get; set; }

       public List<PickupLocationCarModel> Cars { get; set; }

    }
}
