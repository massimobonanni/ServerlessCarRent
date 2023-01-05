using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using ServerlessCarRent.Common.Models.PickupLocation;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ServerlessCarRent.WebSite.Models.PickupLocationsController
{
    public class EditViewModel
    {
        [DisplayName("Location Id")]
        public string Identifier { get; set; }

        [DisplayName("City")]
        public string City { get; set; }

        [DisplayName("Description")]
        public string Location { get; set; }

        [DisplayName("Status")]
        public PickupLocationState Status { get; set; }

        [ValidateNever()]
        public List<SelectListItem> PickupLocationStates { get; set; }
    }
}
