using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;

namespace ServerlessCarRent.Console.Commands;

internal class RootCommand : System.CommandLine.RootCommand
{
    public RootCommand(IServiceProvider serviceProvider) :
        base("Console for Serverless Rent car platform")
    {
        this.Subcommands.Add(new GetCarsManagementCommand(serviceProvider));
        this.Subcommands.Add(new CreateEnvironmentCommand(serviceProvider));
    }
}
