using ServerlessCarRent.Common.Dtos;
using ServerlessCarRent.Common.Models;
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

        public static IEnumerable<object[]> GetRentCarDtos()
        {
            yield return new object[]
            {
                "AA000BB",
                new CarData()
                {
                    CostPerHour=10.0M,
                    Currency="EUR",
                    PickupLocation="MILAN",
                    Model="Fiat 500",
                    CurrentState=CarState.Working,
                     CurrentRentalState=CarRentalState.Free,
                    CurrentRental=null,
                    CurrentRenter=null
                },
                new RentCarDto()
                {
                     RentalId="12345",
                     RenterEmail="jane.doe@mail.com",
                     RenterFirstName="Jane",
                     RenterLastName="Doe",
                     StartDate=DateTimeOffset.Parse("2020-01-01T00:00:00Z"),
                },
                CarRentalState.Rented,
                new CarData()
                {
                    CostPerHour=10.0M,
                    Currency="EUR",
                    PickupLocation="MILAN",
                    Model="Fiat 500",
                    CurrentState=CarState.Working,
                    CurrentRental= new RentalData()
                    {
                         Id="12345",
                         StartDate=DateTimeOffset.Parse("2020-01-01T00:00:00Z"),
                         EndDate=null,
                    },
                    CurrentRenter=new RenterData()
                    {
                        Email="jane.doe@mail.com",
                        FirstName="Jane",
                        LastName="Doe"
                    }
                },
            };
            yield return new object[]
           {
                "AA000BB",
                new CarData()
                {
                    CostPerHour=10.0M,
                    Currency="EUR",
                    PickupLocation="MILAN",
                    Model="Fiat 500",
                    CurrentState=CarState.Working,
                    CurrentRentalState=CarRentalState.Rented,
                    CurrentRental= new RentalData()
                    {
                         Id="12345",
                         StartDate=DateTimeOffset.Parse("2020-01-01T00:00:00Z"),
                         EndDate=null,
                    },
                    CurrentRenter=new RenterData()
                    {
                        Email="jane.doe@mail.com",
                        FirstName="Jane",
                        LastName="Doe"
                    }
                },
                new RentCarDto()
                {
                     RentalId="99999",
                     RenterEmail="john.doe@mail.com",
                     RenterFirstName="John",
                     RenterLastName="Doe",
                     StartDate=DateTimeOffset.Parse("2021-01-01T00:00:00Z"),
                },
                CarRentalState.Rented,
                new CarData()
                {
                    CostPerHour=10.0M,
                    Currency="EUR",
                    PickupLocation="MILAN",
                    Model="Fiat 500",
                    CurrentState=CarState.Working,
                    CurrentRental= new RentalData()
                    {
                         Id="12345",
                         StartDate=DateTimeOffset.Parse("2020-01-01T00:00:00Z"),
                         EndDate=null,
                    },
                    CurrentRenter=new RenterData()
                    {
                        Email="jane.doe@mail.com",
                        FirstName="Jane",
                        LastName="Doe"
                    }
                },
           };
        }
    }
}
