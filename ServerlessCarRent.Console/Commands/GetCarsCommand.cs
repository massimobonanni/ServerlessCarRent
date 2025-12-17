using ServerlessCarRent.RestClient;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Console.Commands
{
    internal class GetCarsCommand : CommandBase
    {
        private readonly Option<string> urlOption;
        private readonly Option<string> keyOption;
        private readonly Option<string> plateOption;
        private readonly Option<string> locationOption;
        private readonly Option<string> modelOption;

        public GetCarsCommand(IServiceProvider serviceProvider) :
            base("search", "Retrieve cars based on filters", serviceProvider)
        {
            urlOption = new Option<string>("--uri")
            {
                Required = true,
                Description = "The service url to call."
            };

            this.Options.Add(urlOption);

            keyOption = new Option<string>("--key")
            {
                Description = "The key to call the service."
            };

            this.Options.Add(keyOption);

            plateOption = new Option<string>("--plate")
            {
                Description = "The plate filter for search cars."
            };

            this.Options.Add(plateOption);

            locationOption = new Option<string>("--location")
            {
                Description = "The location filter for search cars."
            };

            this.Options.Add(locationOption);

            modelOption = new Option<string>("--model")
            {
                Description = "The model filter for search cars."
            };

            this.Options.Add(modelOption);

            this.SetAction(CommandHandler);
        }

        private async Task CommandHandler(ParseResult parseResult, CancellationToken cancellationToken)
        {
            var uri = parseResult.GetRequiredValue(urlOption);
            var key = parseResult.GetValue(keyOption);
            var plate = parseResult.GetValue(plateOption);
            var location = parseResult.GetValue(locationOption);
            var model = parseResult.GetValue(modelOption);

            using var httpClient = new HttpClient();
            var restClient = new CarsManagementClient(httpClient, uri, key);

            var response = await restClient.GetCarsAsync(plate, location, model, null, null);

            if (response != null)
            {
                if (response.Cars.Any())
                {
                    foreach (var item in response.Cars)
                    {
                        System.Console.WriteLine($"[{item.Plate}] - {item.Model} - {item.PickupLocation} - [{item.CurrentStatus},{item.CurrentRentalStatus}]");
                    }
                }
                else
                {
                    var foregroundColor = System.Console.ForegroundColor;
                    System.Console.ForegroundColor = ConsoleColor.Yellow;
                    System.Console.WriteLine("No cars founded");
                    System.Console.ForegroundColor = foregroundColor;
                }
            }
        }
    }
}
