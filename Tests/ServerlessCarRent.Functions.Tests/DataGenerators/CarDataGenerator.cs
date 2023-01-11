using ServerlessCarRent.Common.Models.Car;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Functions.Tests.DataGenerators
{
    internal class CarDataGenerator
    {
        public static IEnumerable<object[]> GetCarDataWithoutDetails()
        {
            yield return new object[]
            {
                "AA000BB",
                new CarData()
                {
                    Model="Fiat 500",
                    PickupLocation="LOCATION01",
                    CostPerHour=10.50M,
                    Currency="EUR",
                    CurrentRental=new Common.Models.RentalData()
                        {
                            Id="11111",
                            StartDate=DateTimeOffset.UtcNow.Subtract(TimeSpan.FromDays(2))
                        },
                    CurrentRentalState=Common.Models.CarRental.CarRentalState.Rented,
                    CurrentRenter= new Common.Models.RenterData()
                        {
                            FirstName="John",
                            LastName="Doe",
                            Email="john.doe@mail.com"
                        },
                    CurrentState=CarState.Working
                }
            };

            yield return new object[]
            {
                "GB424PV",
                new CarData()
                {
                    Model="Toyota CHR",
                    PickupLocation="LOCATION02",
                    CostPerHour=10.50M,
                    Currency="EUR",
                    CurrentRental=null,
                    CurrentRentalState=Common.Models.CarRental.CarRentalState.Free,
                    CurrentRenter= null,
                    CurrentState=CarState.Working
                }
            };
        }
    }
}
