using Microsoft.AspNetCore.Mvc.Rendering;

namespace System.Collections.Generic
{
    public static class CollectionsExtensions
    {
        public static List<SelectListItem> GenerateListItems(this IEnumerable<string> sourceList) 
        {
            var list = new List<SelectListItem>();
            list.AddRange(sourceList.Select(e => new SelectListItem(e, e)));
            return list;
        }

 
    }
}
