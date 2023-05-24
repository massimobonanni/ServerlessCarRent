using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Console.Commands
{
	internal  class GetCarsManagementCommand : Command
	{
        public GetCarsManagementCommand() : base("cars", "manages cars")
        {
            this.Add(new GetCarsCommand());
        }
	}
}
