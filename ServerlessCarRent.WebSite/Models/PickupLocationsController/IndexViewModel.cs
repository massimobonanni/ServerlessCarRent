using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;
using System.ComponentModel;

namespace ServerlessCarRent.WebSite.Models.PickupLocationsController
{
    public class IndexViewModel
    {
        [DisplayName("Location Id")]
        public string IdentifierFilter { get; set; }

        [DisplayName("City")] 
        public string CityFilter { get; set; }

        [DisplayName("Location Description")] 
        public string LocationFilter { get; set; }

        [DisplayName("Location Status")] 
        public IEnumerable<string> StatesFilter { get; set; }


         public List<PickupLocationModel> PickupLocations { get; set; }
    }
}
