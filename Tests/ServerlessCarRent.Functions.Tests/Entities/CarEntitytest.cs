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
    public class CarEntitytest
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

            Assert.NotNull(entityUT.State);
            Assert.Equal(initDto.CarStatus, entityUT.State.CurrentState);
            Assert.Equal(initDto.Model, entityUT.State.Model);
            Assert.Equal(initDto.Currency, entityUT.State.Currency);
            Assert.Equal(initDto.PickupLocation, entityUT.State.PickupLocation);
            Assert.Equal(initDto.CostPerHour, entityUT.State.CostPerHour);
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
            entityUT.State = initialCarState;

            entityUT.Rent(rentDto);

            Assert.NotNull(entityUT.State);
            Assert.Equal(expectedCarState.CurrentState, entityUT.State.CurrentState);
            Assert.Equal(expectedCarState.CurrentRental.Id, entityUT.State.CurrentRental.Id);
            Assert.Equal(expectedCarState.CurrentRental.StartDate, entityUT.State.CurrentRental.StartDate);
            Assert.Null(entityUT.State.CurrentRental.EndDate);
            Assert.Equal(expectedCarState.CurrentRenter.Email, entityUT.State.CurrentRenter.Email);
            Assert.Equal(expectedCarState.CurrentRenter.FirstName, entityUT.State.CurrentRenter.FirstName);
            Assert.Equal(expectedCarState.CurrentRenter.LastName, entityUT.State.CurrentRenter.LastName);
        }
    }
}
