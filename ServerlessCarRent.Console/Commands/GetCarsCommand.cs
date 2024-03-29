﻿using ServerlessCarRent.RestClient;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Console.Commands
{
    internal class GetCarsCommand : Command
    {
        public GetCarsCommand() : base("search", "Retrieve cars based on filters")
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

            var plateOptions = new Option<string>(
               name: "--plate",
               description: "The plate filter for search cars.");

            this.AddOption(plateOptions);

            var locationOptions = new Option<string>(
               name: "--location",
               description: "The location filter for search cars.");

            this.AddOption(locationOptions);

            var modelOptions = new Option<string>(
               name: "--model",
               description: "The model filter for search cars.");

            this.AddOption(modelOptions);

            this.SetHandler(async (uri, key, plate, location, model) =>
            {
                await CommandHandler(uri, key, plate, location, model);
            }, urlOptions, keyOptions, plateOptions, locationOptions, modelOptions);
        }

        private async Task CommandHandler(Uri uri, string key, string plate, string location, string model)
        {
            using var httpClient = new HttpClient();
            var restClient = new CarsManagementClient(httpClient, uri.ToString(), key);

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
