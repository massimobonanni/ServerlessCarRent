using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    internal static class ObjectExtensions
    {
        public static StringContent GenerateStringContent(this object obj) 
        {
            string requestJson = JsonConvert.SerializeObject(obj, Formatting.None);
            return new StringContent(requestJson, Encoding.UTF8, "application/json");
        }
    }
}
