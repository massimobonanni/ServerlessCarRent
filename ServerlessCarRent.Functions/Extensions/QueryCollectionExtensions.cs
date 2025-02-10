using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Http
{
    internal static class QueryCollectionExtensions
    {
        public static string? GetFirstStringValue(this IQueryCollection queryCollection, string key)
        { 
            ArgumentNullException.ThrowIfNull(queryCollection);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(key);

            if (queryCollection.TryGetValue(key, out var values))
            {
                return values.FirstOrDefault();
            }
            return null;
        }
    }
}
