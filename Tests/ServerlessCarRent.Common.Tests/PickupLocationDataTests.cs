using Moq;
using System.Collections.Generic;
using Xunit;

namespace ServerlessCarRent.Common.Models.PickupLocation.Tests
{
    public class PickupLocationDataTests
    {
        [Fact]
        public void CanBeDeleted_ShouldReturnFalse_WhenCarsIsEmpty()
        {
            // Arrange
            var pickupLocationData = new PickupLocationData
            {
                Cars = new List<PickupLocationCarData>()
            };

            // Act
            var result = pickupLocationData.CanBeDeleted();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanBeDeleted_ShouldReturnFalse_WhenCarsIsNull()
        {
            // Arrange
            var pickupLocationData = new PickupLocationData
            {
                Cars = null
            };

            // Act
            var result = pickupLocationData.CanBeDeleted();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanBeDeleted_ShouldReturnTrue_WhenCarsIsNotEmpty()
        {
            // Arrange
            var mockList = new List<PickupLocationCarData> { new PickupLocationCarData() };
            var pickupLocationData = new PickupLocationData
            {
                Cars = mockList
            };

            // Act
            var result = pickupLocationData.CanBeDeleted();

            // Assert
            Assert.False(result);
        }
    }
}
