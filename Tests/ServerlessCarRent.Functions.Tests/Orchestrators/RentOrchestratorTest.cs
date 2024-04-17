using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Moq;
using ServerlessCarRent.Common.Dtos;
using ServerlessCarRent.Common.Interfaces;
using ServerlessCarRent.Functions.Clients;
using ServerlessCarRent.Functions.Entities;
using ServerlessCarRent.Functions.Orchestrators;
using ServerlessCarRent.Functions.Tests.DataGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Functions.Tests.Orchestrators
{
    public class RentOrchestratorTest
    {
        [Theory]
        [MemberData(nameof(RentOrchestratorDataGenerator.GetData), MemberType = typeof(RentOrchestratorDataGenerator))]
        public async void RunOrchestrator_Test(RentOrchestratorDto orchestratorDto, string instanceId,
            bool pickupLocationRentResult, RentOperationState expectedOrchestratorResult)
        {
            // Setup Ilogger
            var loggerMock = new Mock<ILogger<RentOrchestrator>>();

            // Setup IDurableOrchestrationContext
            var durableContextMock = new Mock<IDurableOrchestrationContext>();
            durableContextMock.Setup(e => e.GetInput<RentOrchestratorDto>())
                .Returns(orchestratorDto);
            var pickupLocationEntityId = new EntityId(nameof(PickupLocationEntity), orchestratorDto.PickupLocation);
            durableContextMock.Setup(e => e.CallEntityAsync<bool>(pickupLocationEntityId, nameof(IPickupLocationEntity.RentCar), It.IsAny<RentCarPickupLocationDto>()))
                .Returns(Task.FromResult(pickupLocationRentResult));
            durableContextMock.SetupGet(e => e.InstanceId)
                .Returns(instanceId);

            var orchestratorUT = new RentOrchestrator(loggerMock.Object);

            var actual = await orchestratorUT.RunOrchestrator(durableContextMock.Object);

            Assert.NotNull(actual);
            Assert.Equal(orchestratorDto.CarPlate, actual.CarPlate);
            Assert.Equal(orchestratorDto.PickupLocation, actual.PickupLocation);
            Assert.Equal(expectedOrchestratorResult, actual.State);
            Assert.Equal(instanceId, actual.RentalId);
        }
    }
}
