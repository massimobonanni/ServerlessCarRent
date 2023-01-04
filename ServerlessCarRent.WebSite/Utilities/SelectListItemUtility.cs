using Microsoft.AspNetCore.Mvc.Rendering;
using ServerlessCarRent.Common.Models.Car;

namespace ServerlessCarRent.WebSite.Utilities
{
    public static class SelectListItemUtility
    {
        public static List<SelectListItem> GenerateListFromCarStates(CarState? selectedState = null)
        {
            var list = new List<SelectListItem>();
            foreach (var state in Enum.GetValues(typeof(CarState)))
            {
                list.Add(new SelectListItem(state.ToString(),
                    ((int)state).ToString(),
                    selectedState.HasValue && selectedState.Value== (CarState)state));
            }
            return list;
        }
    }
}
