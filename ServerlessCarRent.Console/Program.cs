using Microsoft.Extensions.DependencyInjection;
using ServerlessCarRent.Console.Commands;
using System.CommandLine;

var serviceCollection = new ServiceCollection();
var serviceProvider = serviceCollection.BuildServiceProvider();

var rootCommand = new ServerlessCarRent.Console.Commands.RootCommand(serviceProvider);

ParseResult parseResult = rootCommand.Parse(args);
return await parseResult.InvokeAsync();

