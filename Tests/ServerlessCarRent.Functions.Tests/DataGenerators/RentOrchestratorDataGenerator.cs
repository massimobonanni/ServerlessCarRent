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
    internal class RentOrchestratorDataGenerator
    {
        public static IEnumerable<object[]> GetData()
        {
            yield return new object[]
            {
                new RentOrchestratorDto()
                {
                     CarPlate="AA000AA",
                     PickupLocation="LOCATION01",
                     RentalStartDate=DateTimeOffset.Now,
                     RenterEmail="jane.doe@mail.com",
                     RenterFirstName="Jane",
                     RenterLastName="Doe"
                },
                "1234567890",
                true,
                RentOperationState.Complete
            };

            yield return new object[]
            {
                new RentOrchestratorDto()
                {
                     CarPlate="AA000AA",
                     PickupLocation="LOCATION01",
                     RentalStartDate=DateTimeOffset.Now,
                     RenterEmail="jane.doe@mail.com",
                     RenterFirstName="Jane",
                     RenterLastName="Doe"
                },
                "1234567890",
                false,
                RentOperationState.Error
            };

        }

    }
}
