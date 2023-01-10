using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.PickupLocation;
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

        public bool LocationOpenFilter { get; set; }
        public bool LocationClosedFilter { get; set; }

        public IEnumerable<PickupLocationState> StatesFilter
        {
            get
            {
                var result = new List<PickupLocationState>();
                if (this.LocationOpenFilter)
                    result.Add(PickupLocationState.Open);
                if (this.LocationClosedFilter)
                    result.Add(PickupLocationState.Closed);
                return result;
            }
            set
            {
                this.LocationOpenFilter = value.Contains(PickupLocationState.Open);
                this.LocationClosedFilter = value.Contains(PickupLocationState.Closed);
            }
        }


        public List<PickupLocationModel> PickupLocations { get; set; }

    }
}
