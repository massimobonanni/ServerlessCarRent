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

            Assert.NotNull(entityUT.Status);
            Assert.Equal(initDto.CarStatus, entityUT.Status.CurrentState);
            Assert.Equal(initDto.Model, entityUT.Status.Model);
            Assert.Equal(initDto.Currency, entityUT.Status.Currency);
            Assert.Equal(initDto.PickupLocation, entityUT.Status.PickupLocation);
            Assert.Equal(initDto.CostPerHour, entityUT.Status.CostPerHour);
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
            entityUT.Status = initialCarState;

            entityUT.Rent(rentDto);

            Assert.NotNull(entityUT.Status);
            Assert.Equal(expectedCarState.CurrentState, entityUT.Status.CurrentState);
            Assert.Equal(expectedCarState.CurrentRental.Id, entityUT.Status.CurrentRental.Id);
            Assert.Equal(expectedCarState.CurrentRental.StartDate, entityUT.Status.CurrentRental.StartDate);
            Assert.Null(entityUT.Status.CurrentRental.EndDate);
            Assert.Equal(expectedCarState.CurrentRenter.Email, entityUT.Status.CurrentRenter.Email);
            Assert.Equal(expectedCarState.CurrentRenter.FirstName, entityUT.Status.CurrentRenter.FirstName);
            Assert.Equal(expectedCarState.CurrentRenter.LastName, entityUT.Status.CurrentRenter.LastName);
        }
    }
}
