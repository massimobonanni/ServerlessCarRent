using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.RestClient
{
    public class ClientResult
    {
        public bool Succeeded { get; set; }

        public List<string> Errors { get; set; } = new List<string>();
    }
}
