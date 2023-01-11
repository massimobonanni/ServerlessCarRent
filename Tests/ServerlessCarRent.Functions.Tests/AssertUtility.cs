using ServerlessCarRent.Common.Models.Car;
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
    }
}
