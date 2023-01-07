using Microsoft.AspNetCore.Mvc.Rendering;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.PickupLocation;
using ServerlessCarRent.RestClient;

namespace ServerlessCarRent.WebSite.Utilities
{
    public static class SelectListItemUtility
    {
        public static List<SelectListItem> GenerateListFromCarStates(CarState? selectedState = null,
            bool addEmptyRow = false)
        {
            var list = new List<SelectListItem>();
            
            if (addEmptyRow)
                 list.Add(new SelectListItem(string.Empty, null));
 
            foreach (var state in Enum.GetValues(typeof(CarState)))
            {
                list.Add(new SelectListItem(state.ToString(),
                    ((int)state).ToString(),
                    selectedState.HasValue && selectedState.Value== (CarState)state));
            }
            return list;
        }

        public static List<SelectListItem> GenerateListFromPickupLocationStates(PickupLocationState? selectedState = null,
            bool addEmptyRow = false)
        {
            var list = new List<SelectListItem>();

            if (addEmptyRow)
                list.Add(new SelectListItem(string.Empty, null));

            foreach (var state in Enum.GetValues(typeof(PickupLocationState)))
            {
                list.Add(new SelectListItem(state.ToString(),
                    ((int)state).ToString(),
                    selectedState.HasValue && selectedState.Value == (PickupLocationState)state));
            }
            return list;
        }

        public static async Task<List<SelectListItem>> GenerateListFromPickupLocationsAsync(
            PickupLocationsManagementClient client,
            string selectedLocation = null,
            bool addEmptyRow = false)
        {
            var list = new List<SelectListItem>();

            if (addEmptyRow)
                list.Add(new SelectListItem(string.Empty, null));

            var pickupLocations = await client.GetPickupLocationsAsync(null, null, null, null);

            foreach (var location in pickupLocations.PickupLocations.OrderBy(p=>p.Location))
            {
                list.Add(new SelectListItem(location.Location,
                    location.Identifier,
                    selectedLocation!=null && selectedLocation == location.Identifier));
            }
            return list;
        }
    }
}
