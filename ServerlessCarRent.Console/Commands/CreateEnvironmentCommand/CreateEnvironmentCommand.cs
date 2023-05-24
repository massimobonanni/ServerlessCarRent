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
    internal class CreateEnvironmentCommand:Command
    {
        public CreateEnvironmentCommand() : 
            base("createenv", "Creates pickup locations and cars based on input file (JSON)")
        {
            var urlOptions = new Option<Uri>(
                name: "--uri",
                description: "The service url to call.")
            { IsRequired = true };

            this.AddOption(urlOptions);

            var keyOptions = new Option<string>(
                name: "--key",
                description: "The key to call the service.");

            this.AddOption(keyOptions);

            var fileOptions = new Option<string>(
               name: "--file",
               description: "The JSON file full path to use for configuration.");

            this.AddOption(fileOptions);

            var createJsonOptions = new Option<bool>(
               "--createJson",
               () => false,
               "Save a templet for the JSON file");

            this.AddOption(createJsonOptions);

            this.SetHandler(async (uri, key, file, createJson) =>
            {
                await CommandHandler(uri, key, file, createJson);
            }
            , urlOptions, keyOptions, fileOptions, createJsonOptions);

        }

        private async Task CommandHandler(Uri uri, string key, string file, bool createJson)
        {
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
            var pickupLocationsClient = new PickupLocationsManagementClient(httpClient, uri.ToString(), key);

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
                var locationResponse=await pickupLocationsClient.CreatePickupLocationAsync(location);
                System.Console.WriteLine($"Location '{item.identifier}' created with result {locationResponse.Succeeded}");
            }

            // Cars creation
            var carsClient = new CarsManagementClient(httpClient, uri.ToString(), key);

            foreach (var item in data.cars)
            {
                var car = new InitializeCarRequest()
                {
                    Plate=item.plate,
                    Model=item.model,
                    PickupLocation=item.location,
                    CostPerHour=item.costPerHour,
                    Currency=item.currency,
                    CurrentStatus=Common.Models.Car.CarState.Working
                };

                System.Console.WriteLine($"Creation car '{item.plate}'");
                var carResponse = await carsClient.CreateCarAsync(car);
                System.Console.WriteLine($"Car '{item.plate}' created with result {carResponse.Succeeded}");
            }

            System.Console.WriteLine($"Import complete");
        }
    }
}
