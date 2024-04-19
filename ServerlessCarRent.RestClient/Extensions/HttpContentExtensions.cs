using Newtonsoft.Json;
using ServerlessCarRent.Common.Models.Car;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Http
{
    internal static class HttpContentExtensions
    {
        public static async Task<T> DeserializeObjectAsync<T>(this HttpContent content)
        {
            var contentAsString = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(contentAsString);
        }
    }
}
