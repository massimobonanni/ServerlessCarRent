using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Console.Commands.CreateEnvironment
{
    internal class Car
    {
        public string plate { get; set; }
        public string model { get; set; }
        public string location { get; set; }
        public decimal costPerHour { get; set; }
        public string currency { get; set; }
    }

    internal class PickupLocation
    {
        public string identifier { get; set; }
        public string city { get; set; }
        public string location { get; set; }
    }

    internal class JsonEnvironment
    {
        public List<PickupLocation> pickupLocations { get; set; }
        public List<Car> cars { get; set; }

        public static JsonEnvironment GetTemplateData()
        {
            var data = new JsonEnvironment();

            data.pickupLocations = new List<PickupLocation>
            {
                new PickupLocation()
                {
                    identifier = "LOCATION-1",
                    city = "Rome",
                    location = "Location 1"
                },
                new PickupLocation()
                {
                    identifier = "LOCATION-2",
                    city = "Milan",
                    location = "Location 2"
                }
            };

            data.cars = new List<Car>
            {
                new Car()
                {
                    plate = "AA000AA",
                    model = "Fiat 500",
                    location = "LOCATION-1",
                    costPerHour = 10.50M,
                    currency = "EUR"
                },
                new Car()
                {
                    plate = "BB111BB",
                    model = "Ford Focus",
                    location = "LOCATION-2",
                    costPerHour = 20.75M,
                    currency = "EUR"
                }
            };

            return data;
        }
    }
}
