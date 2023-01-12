using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.CarRental;
using ServerlessCarRent.Functions.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Functions.Tests
{
    internal static class AssertUtility
    {
        public static bool AreCarsEqual(CarData expected, GetCarResponse actual)
        {
            var result = true;

            result&= expected.CostPerHour == actual.CostPerHour;
            result &= expected.Currency == actual.Currency;
            result &= expected.CurrentRentalState == actual.CurrentRentalState;
            result &= expected.CurrentState == actual.CurrentState;
            if (expected.CurrentRenter != null)
            {
                result &= expected.CurrentRenter.Email == actual.CurrentRenterEmail;
                result &= expected.CurrentRenter.FirstName == actual.CurrentRenterFirstName;
                result &= expected.CurrentRenter.LastName == actual.CurrentRenterLastName;
            }

            return result;
        }

        public static bool AreCarRentalsEqual(CarRentalsData expected, GetCarResponse actual)
        {
            var result = true;
            result &= expected.Rentals.Count == actual.Rentals.Count;

            foreach (var item in actual.Rentals)
            {
                var expectedItem = expected.Rentals.FirstOrDefault(i => i.Rental.Id == item.RentalId);
                result &= expectedItem != null;
                result &= expectedItem?.CostPerHour == item.CostPerHour;
                result &= expectedItem?.Renter.Email == item.RenterEmail;
                result &= expectedItem?.Renter.FirstName== item.RenterFirstName;
                result &= expectedItem?.Renter.LastName== item.RenterLastName;
                result &= expectedItem?.Rental.StartDate == item.RentalStart;
                result &= expectedItem?.Rental.EndDate == item.RentalEnd;
            }

            return result;
        }
    }
}
