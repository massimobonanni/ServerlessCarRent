using ServerlessCarRent.Common.Dtos;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.CarRental;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Functions.Tests.DataGenerators
{
    internal class CarEntityDataGenerator
    {
        public static IEnumerable<object[]> GetInitializeCarDtos()
        {
            yield return new object[]
            {
                "AA000BB",
                new InitializeCarDto()
                {
                    CarStatus = CarState.Working,
                    CostPerHour = 10M,
                    Currency = "EUR",
                    Model = "Fiat 500",
                    PickupLocation = "MILAN"
                }
            };
            yield return new object[]
            {
                "CC111DD",
                new InitializeCarDto()
                {
                    CarStatus = CarState.Working,
                    CostPerHour = 10M,
                    Currency = "EUR",
                    Model = "Fiat 500",
                    PickupLocation = null
                }
            };
        }
    }
}
