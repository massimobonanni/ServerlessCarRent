using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    internal static class ObjectExtensions
    {
        public static IActionResult CreateOkResponse(this object source)
        {
            var objSerialization = JsonConvert.SerializeObject(source);
            return new OkObjectResult(objSerialization);
        }

        public static IActionResult CreateOkResponse(this object source, int httpStatusCode)
        {
            var objSerialization = JsonConvert.SerializeObject(source);
            return new OkObjectResult(objSerialization) { StatusCode = httpStatusCode };
        }
    }
}
