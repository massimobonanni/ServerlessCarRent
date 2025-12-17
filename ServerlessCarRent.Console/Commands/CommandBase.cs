using System.CommandLine;

namespace ServerlessCarRent.Console.Commands;

internal abstract class CommandBase : Command
{
    protected readonly IServiceProvider serviceProvider;

    public CommandBase(string name, string description, IServiceProvider serviceProvider) :
        base(name, description)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        this.serviceProvider = serviceProvider;
    }
}
