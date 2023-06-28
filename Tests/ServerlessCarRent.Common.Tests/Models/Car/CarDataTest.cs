using System;
using Xunit;
using Moq;
using ServerlessCarRent.Common.Models.CarRental;

namespace ServerlessCarRent.Common.Models.Car.Tests
{
    public class CarDataTest
    {
        [Fact]
        public void CanBeRent_ReturnsFalse_WhenRentalStateIsRented()
        {
            // Arrange
            var carData = new CarData() { CurrentRentalState = CarRentalState.Rented };

            // Act
            var result = carData.CanBeRent();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanBeRent_ReturnsFalse_WhenRentalStateIsUnknown()
        {
            // Arrange
            var carData = new CarData() { CurrentRentalState = CarRentalState.Unknown };

            // Act
            var result = carData.CanBeRent();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanBeRent_ReturnsFalse_WhenCarStateIsUnusable()
        {
            // Arrange
            var carData = new CarData() { CurrentRentalState = CarRentalState.Free, CurrentState = CarState.Unusable };

            // Act
            var result = carData.CanBeRent();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanBeRent_ReturnsFalse_WhenCarStateIsUnderMaintenance()
        {
            // Arrange
            var carData = new CarData() { CurrentRentalState = CarRentalState.Free, CurrentState = CarState.UnderMaintenance };

            // Act
            var result = carData.CanBeRent();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanBeRent_ReturnsTrue_WhenCanBeRent()
        {
            // Arrange
            var carData = new CarData() { CurrentRentalState = CarRentalState.Free, CurrentState = CarState.Working };

            // Act
            var result = carData.CanBeRent();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanBeReturn_ReturnsFalse_WhenRentalStateIsNotRented()
        {
            // Arrange
            var carData = new CarData() { CurrentRentalState = CarRentalState.Free };

            // Act
            var result = carData.CanBeReturn();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanBeReturn_ReturnsTrue_WhenRentalStateIsRented()
        {
            // Arrange
            var carData = new CarData() { CurrentRentalState = CarRentalState.Rented };

            // Act
            var result = carData.CanBeReturn();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanBeDeleted_ReturnsFalse_WhenRentalStateIsNotFreeOrUnknown()
        {
            // Arrange
            var carData = new CarData() { CurrentRentalState = CarRentalState.Rented };

            // Act
            var result = carData.CanBeDeleted();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanBeDeleted_ReturnsTrue_WhenRentalStateIsFree()
        {
            // Arrange
            var carData = new CarData() { CurrentRentalState = CarRentalState.Free };

            // Act
            var result = carData.CanBeDeleted();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanBeDeleted_ReturnsTrue_WhenRentalStateIsUnknown()
        {
            // Arrange
            var carData = new CarData() { CurrentRentalState = CarRentalState.Unknown };

            // Act
            var result = carData.CanBeDeleted();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CalculateCost_ReturnsZero_WhenCurrentRentalIsNull()
        {
            // Arrange
            var carData = new CarData() { CurrentRental = null };

            // Act
            var result = carData.CalculateCost();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void CalculateCost_ReturnsExpectedCost_WhenCurrentRentalIsNotNull()
        {
            // Arrange
            var carData = new CarData() { CurrentRental = new RentalData() { StartDate = DateTimeOffset.Now.Subtract(TimeSpan.FromHours(2)), EndDate = DateTimeOffset.Now }, CostPerHour = 10.50M };

            // Act
            var result = carData.CalculateCost();

            // Assert
            Assert.Equal(21, result);
        }

        [Fact]
        public void CanBeRent_ReturnsTrue_WhenCarStateIsWorkingAndRentalStateIsFree_Mocked()
        {
            // Arrange
            var carData = new CarData() { 
                CurrentState=CarState.Working,
                CurrentRentalState=CarRentalState.Free,
                CurrentRental = null, 
                CostPerHour = 10.50M };

            // Act
            var result = carData.CanBeRent();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanBeRent_ReturnsFalse_WhenCarStateIsUnusableAndRentalStateIsFree_Mocked()
        {
            // Arrange
            var carData = new CarData()
            {
                CurrentState = CarState.Unusable,
                CurrentRentalState = CarRentalState.Free,
                CurrentRental = null,
                CostPerHour = 10.50M
            };

            // Act
            var result = carData.CanBeRent();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanBeRent_ReturnsFalse_WhenCarStateIsUnderMaintenanceAndRentalStateIsFree_Mocked()
        {
            // Arrange
            var carData = new CarData()
            {
                CurrentState = CarState.UnderMaintenance,
                CurrentRentalState = CarRentalState.Free,
                CurrentRental = null,
                CostPerHour = 10.50M
            };

            // Act
            var result = carData.CanBeRent();

            // Assert
            Assert.False(result);
        }
    }
}
