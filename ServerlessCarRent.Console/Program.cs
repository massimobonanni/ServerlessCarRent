using ServerlessCarRent.Console.Commands;
using System.CommandLine;

namespace scl;

class Program
{
	static async Task<int> Main(string[] args)
	{
		var rootCommand = new RootCommand("Console for Serverless Rent car platform");
		
		rootCommand.AddCommand(new GetCarsManagementCommand());
        rootCommand.AddCommand(new CreateEnvironmentCommand());

        return await rootCommand.InvokeAsync(args);
	}

		
}
