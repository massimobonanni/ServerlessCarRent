using ServerlessCarRent.Common.Dtos;
using ServerlessCarRent.Functions.Tests.DataGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Logging;
using ServerlessCarRent.Functions.Orchestrators;
using ServerlessCarRent.Functions.Entities;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using ServerlessCarRent.Common.Interfaces;
using ServerlessCarRent.Common.Models.CarRental;
using ServerlessCarRent.Common.Models.Car;

namespace ServerlessCarRent.Functions.Tests.Entities
{
    public class CarEntityTest
    {
        [Theory]
        [MemberData(nameof(CarEntityDataGenerator.GetInitializeCarDtos), MemberType = typeof(CarEntityDataGenerator))]
        public void Initialize_Test(string carPlate, InitializeCarDto initDto)
        {
            // Setup Ilogger
            var loggerMock = new Mock<ILogger<CarEntity>>();
            // Setup IDurableEntityContext
            var entityContextMock = new Mock<IDurableEntityContext>();
            entityContextMock.SetupGet(e => e.EntityKey).Returns(carPlate);
            if (!string.IsNullOrEmpty(initDto.PickupLocation))
            {
                var pickupLocationEntityId = new EntityId(nameof(PickupLocationEntity), initDto.PickupLocation);
                entityContextMock.Setup(e => e.SignalEntity(pickupLocationEntityId,
                        nameof(IPickupLocationEntity.CarStatusChanged), It.IsAny<CarStatusChangeDto>()))
                    .Verifiable();
            }
            Entity.SetMockContext(entityContextMock.Object);

            var entityUT = new CarEntity(loggerMock.Object);

            entityUT.Initialize(initDto);

            var stateAssert = entityUT.GetState();
            Assert.NotNull(stateAssert);
            Assert.Equal(initDto.CarStatus, stateAssert.CurrentState);
            Assert.Equal(initDto.Model, stateAssert.Model);
            Assert.Equal(initDto.Currency, stateAssert.Currency);
            Assert.Equal(initDto.PickupLocation, stateAssert.PickupLocation);
            Assert.Equal(initDto.CostPerHour, stateAssert.CostPerHour);
        }


        [Theory]
        [MemberData(nameof(CarEntityDataGenerator.GetRentCarDtos), MemberType = typeof(CarEntityDataGenerator))]
        public void Rent_Test(string carPlate, CarData initialCarState,
            RentCarDto rentDto, CarRentalState expectedStatus, CarData expectedCarState)
        {
            // Setup Ilogger
            var loggerMock = new Mock<ILogger<CarEntity>>();
            // Setup IDurableEntityContext
            var entityContextMock = new Mock<IDurableEntityContext>();
            entityContextMock.SetupGet(e => e.EntityKey).Returns(carPlate);
            if (initialCarState.CanBeRent())
            {
                entityContextMock.Setup(e => e.StartNewOrchestration(nameof(RentalStatusChangeOrchestrator),
                    It.IsAny<RentalStatusChangeOrchestratorDto>(), null))
                    .Verifiable();
            }
            Entity.SetMockContext(entityContextMock.Object);

            var entityUT = new CarEntity(loggerMock.Object);
            entityUT.SetState(initialCarState);

            entityUT.Rent(rentDto);

            var stateAssert = entityUT.GetState();
            Assert.NotNull(stateAssert);
            Assert.Equal(expectedCarState.CurrentState, stateAssert.CurrentState);
            Assert.Equal(expectedCarState.CurrentRental.Id, stateAssert.CurrentRental.Id);
            Assert.Equal(expectedCarState.CurrentRental.StartDate, stateAssert.CurrentRental.StartDate);
            Assert.Null(stateAssert.CurrentRental.EndDate);
            Assert.Equal(expectedCarState.CurrentRenter.Email, stateAssert.CurrentRenter.Email);
            Assert.Equal(expectedCarState.CurrentRenter.FirstName, stateAssert.CurrentRenter.FirstName);
            Assert.Equal(expectedCarState.CurrentRenter.LastName, stateAssert.CurrentRenter.LastName);
        }
    }
}
