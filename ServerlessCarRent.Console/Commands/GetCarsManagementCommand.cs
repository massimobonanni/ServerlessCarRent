using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Console.Commands
{
	internal  class GetCarsManagementCommand : CommandBase
	{
        public GetCarsManagementCommand(IServiceProvider serviceProvider) : 
            base("cars", "manages cars",serviceProvider)
        {
            this.Add(new GetCarsCommand(serviceProvider));
        }
	}
}
