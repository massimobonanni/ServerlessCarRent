using Newtonsoft.Json;
using ServerlessCarRent.Console.Commands.CreateEnvironment;
using ServerlessCarRent.Functions.Requests;
using ServerlessCarRent.RestClient;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Console.Commands
{
    internal class CreateEnvironmentCommand : CommandBase
    {
        private readonly Option<string> urlOption;
        private readonly Option<string> keyOption;
        private readonly Option<string> fileOption;
        private readonly Option<bool> createJsonOption;

        public CreateEnvironmentCommand(IServiceProvider serviceProvider) :
            base("createenv", "Creates pickup locations and cars based on input file (JSON)", serviceProvider)
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

            fileOption = new Option<string>("--file")
            {
                Description = "The JSON file full path to use for configuration."
            };

            this.Options.Add(fileOption);

            createJsonOption = new Option<bool>("--createJson")
            {
                Description = "Create a template for the JSON file.",
                DefaultValueFactory = (a) => false
            };

            this.Options.Add(createJsonOption);

            this.SetAction(CommandHandler);

        }

        private async Task CommandHandler(ParseResult parseResult, CancellationToken cancellationToken)
        {
            var uri = parseResult.GetRequiredValue(urlOption);
            var file = parseResult.GetValue(fileOption) ?? "environment.json";
            var key = parseResult.GetValue(keyOption);
            var createJson = parseResult.GetValue(createJsonOption);

            JsonEnvironment? data;
            if (createJson)
            {
                data = JsonEnvironment.GetTemplateData();

                System.Console.WriteLine($"Creating file '{file}'");
                await File.WriteAllTextAsync(file, JsonConvert.SerializeObject(data, Formatting.Indented));
                System.Console.WriteLine($"Export complete");

                return;
            }

            System.Console.WriteLine($"Loading file '{file}'");

            var fileContent = await File.ReadAllTextAsync(file);
            data = JsonConvert.DeserializeObject<JsonEnvironment>(fileContent);

            using var httpClient = new HttpClient();

            // Pickup Locations creation
            var pickupLocationsClient = new PickupLocationsManagementClient(httpClient, uri, key);

            foreach (var item in data.pickupLocations)
            {
                var location = new InitializePickupLocationRequest()
                {
                    City = item.city,
                    Identifier = item.identifier,
                    Location = item.location,
                    Status = Common.Models.PickupLocation.PickupLocationState.Open
                };

                System.Console.WriteLine($"Creation pickup location '{item.identifier}'");
                var locationResponse = await pickupLocationsClient.CreatePickupLocationAsync(location);
                System.Console.WriteLine($"Location '{item.identifier}' created with result {locationResponse.Succeeded}");
            }

            // Cars creation
            var carsClient = new CarsManagementClient(httpClient, uri.ToString(), key);

            foreach (var item in data.cars)
            {
                var car = new InitializeCarRequest()
                {
                    Plate = item.plate,
                    Model = item.model,
                    PickupLocation = item.location,
                    CostPerHour = item.costPerHour,
                    Currency = item.currency,
                    CurrentStatus = Common.Models.Car.CarState.Working
                };

                System.Console.WriteLine($"Creation car '{item.plate}'");
                var carResponse = await carsClient.CreateCarAsync(car);
                System.Console.WriteLine($"Car '{item.plate}' created with result {carResponse.Succeeded}");
            }

            System.Console.WriteLine($"Import complete");
        }
    }
}
