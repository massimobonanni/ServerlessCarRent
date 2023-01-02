using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Console.Commands
{
	internal static class GetCarsManagementCommand
	{
		public static Command GetCommand()
		{
			var carsManagementCommand = new Command("cars", "manages cars");

			carsManagementCommand.Add(GetCarsCommand.GetCommand());

			return carsManagementCommand;
		}
	}
}
