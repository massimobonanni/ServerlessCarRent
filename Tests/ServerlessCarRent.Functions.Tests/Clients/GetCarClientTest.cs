using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.CarRental;
using ServerlessCarRent.Functions.Clients;
using ServerlessCarRent.Functions.Entities;
using ServerlessCarRent.Functions.Responses;
using ServerlessCarRent.Functions.Tests.DataGenerators;
using ServerlessCarRent.Functions.Tests.Mocks;

namespace ServerlessCarRent.Functions.Tests.Clients
{
    public class GetCarClientTest
    {
        [Fact]
        public async Task Run_ShouldReturnNotFound_WhenPlateNotExist_AndNoDetailRequest()
        {
            var plate = "AA000AA";
            // Setup Ilogger
            var loggerMock = new Mock<ILogger<GetCarClient>>();
            //Setup HttpRequest            
            var requestMock = HttpRequestMockUtility.CreateMockForGetRequest(null);
            // Setup IDurableEntityClient
            var durableClientMock = new Mock<DurableTaskClient>();
            var entityId = new EntityInstanceId(nameof(CarEntity), plate);
            durableClientMock.Setup(c => c.Entities.GetEntityAsync<JObject>(entityId,true,default))
                .Returns(() => Task.FromResult(new EntityStateResponse<JObject>() { EntityState = null, EntityExists = false }));

            // Create Class to test
            var funcClass = new GetCarClient(loggerMock.Object);


            // Call method under test
            var actual = await funcClass.Run(requestMock.Object, plate, durableClientMock.Object);

            // Assertions
            Assert.NotNull(actual);
            Assert.IsType<NotFoundResult>(actual);
        }

        [Fact]
        public async Task Run_ShouldReturnNotFound_WhenPlateNotExist_AndDetailRequest()
        {
            var plate = "AA000AA";
            // Setup Ilogger
            var loggerMock = new Mock<ILogger<GetCarClient>>();
            //Setup HttpRequest
            var requestMock = HttpRequestMockUtility.CreateMockForGetRequest("details");
            // Setup IDurableEntityClient
            var durableClientMock = new Mock<DurableTaskClient>();
            var entityId = new EntityInstanceId(nameof(CarEntity), plate);
            durableClientMock.Setup(c => c.Entities.GetEntityAsync<JObject>(entityId, true, default))
                .Returns(() => Task.FromResult(new EntityStateResponse<JObject>() { EntityState = null, EntityExists = false }));

            // Create Class to test
            var funcClass = new GetCarClient(loggerMock.Object);


            // Call method under test
            var actual = await funcClass.Run(requestMock.Object, plate, durableClientMock.Object);

            // Assertions
            Assert.NotNull(actual);
            Assert.IsType<NotFoundResult>(actual);
        }

        [Theory]
        [MemberData(nameof(CarDataGenerator.GetCarDataWithoutDetails), MemberType = typeof(CarDataGenerator))]
        public async Task Run_ShouldReturnOk_WhenPlateExist_AndNoDetailRequest(string plate, CarData carData)
        {
            // Setup Ilogger
            var loggerMock = new Mock<ILogger<GetCarClient>>();
            //Setup HttpRequest            
            var requestMock = HttpRequestMockUtility.CreateMockForGetRequest(null);
            // Setup IDurableEntityClient
            var durableClientMock = new Mock<DurableTaskClient>();
            var entityId = new EntityInstanceId(nameof(CarEntity), plate);
            durableClientMock.Setup(c => c.Entities.GetEntityAsync<JObject>(entityId, true, default))
                .Returns(() =>
                {
                    var entityState = JObject.FromObject(new { Status = carData });
                    return Task.FromResult(new EntityStateResponse<JObject>() { EntityState = entityState, EntityExists = true });
                });

            // Create Class to test
            var funcClass = new GetCarClient(loggerMock.Object);


            // Call method under test
            var actual = await funcClass.Run(requestMock.Object, plate, durableClientMock.Object);

            // Assertions
            Assert.NotNull(actual);
            Assert.IsType<OkObjectResult>(actual);
            var responseValue = ((OkObjectResult)actual).Value;
            Assert.IsType<GetCarResponse>(responseValue);
            Assert.Equal(plate, ((GetCarResponse)responseValue).Plate);
            AssertUtility.AreCarsEqual(carData, ((GetCarResponse)responseValue));
        }

        [Theory]
        [MemberData(nameof(CarDataGenerator.GetCarDataWithDetails), MemberType = typeof(CarDataGenerator))]
        public async Task Run_ShouldReturnOk_WhenPlateExist_WithDetailRequest(string plate, CarData carData, CarRentalsData carRentals)
        {
            // Setup Ilogger
            var loggerMock = new Mock<ILogger<GetCarClient>>();
            //Setup HttpRequest            
            var requestMock = HttpRequestMockUtility.CreateMockForGetRequest("details");
            // Setup IDurableEntityClient
            var durableClientMock = new Mock<DurableTaskClient>();
            var entityId = new EntityInstanceId(nameof(CarEntity), plate);
            durableClientMock.Setup(c => c.Entities.GetEntityAsync<JObject>(entityId, true, default))
                .Returns(() =>
                {
                    var entityState = JObject.FromObject(new { Status = carData });
                    return Task.FromResult(new EntityStateResponse<JObject>() { EntityState = entityState, EntityExists = true });
                });
            var rentalsEntityId = new EntityInstanceId(nameof(CarRentalsEntity), plate);
            durableClientMock.Setup(c => c.Entities.GetEntityAsync<JObject>(rentalsEntityId, true, default))
                .Returns(() =>
                {
                    var entityState = JObject.FromObject(new { Status = carRentals });
                    return Task.FromResult(new EntityStateResponse<JObject>() { EntityState = entityState, EntityExists = true });
                });
            // Create Class to test
            var funcClass = new GetCarClient(loggerMock.Object);


            // Call method under test
            var actual = await funcClass.Run(requestMock.Object, plate, durableClientMock.Object);

            // Assertions
            Assert.NotNull(actual);
            Assert.IsType<OkObjectResult>(actual);
            var responseValue = ((OkObjectResult)actual).Value;
            Assert.IsType<GetCarResponse>(responseValue);
            Assert.Equal(plate, ((GetCarResponse)responseValue).Plate);
            AssertUtility.AreCarsEqual(carData, (GetCarResponse)responseValue);
            AssertUtility.AreCarRentalsEqual(carRentals, (GetCarResponse)responseValue);
        }
    }
}
