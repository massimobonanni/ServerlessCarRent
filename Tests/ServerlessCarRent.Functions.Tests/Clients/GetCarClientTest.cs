using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.CarRental;
using ServerlessCarRent.Functions.Clients;
using ServerlessCarRent.Functions.Entities;
using ServerlessCarRent.Functions.Responses;
using ServerlessCarRent.Functions.Tests.DataGenerators;
using ServerlessCarRent.Functions.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core.Tokens;

namespace ServerlessCarRent.Functions.Tests.Clients
{
    public class GetCarClientTest
    {
        [Fact]
        public async void Run_PlateNotExist_NoDetail_ReturnNotFound()
        {
            var plate = "AA000AA";
            // Setup Ilogger
            var loggerMock = new Mock<ILogger<GetCarClient>>();
            //Setup HttpRequest            
            var requestMock = HttpRequestMockUtility.CreateMockForGetRequest(null);
            // Setup IDurableEntityClient
            var durableClientMock = new Mock<IDurableEntityClient>();
            var entityId = new EntityId(nameof(CarEntity), plate);
            durableClientMock.Setup(c => c.ReadEntityStateAsync<JObject>(entityId, null, null))
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
        public async void Run_PlateNotExist_WithDetail_ReturnNotFound()
        {
            var plate = "AA000AA";
            // Setup Ilogger
            var loggerMock = new Mock<ILogger<GetCarClient>>();
            //Setup HttpRequest
            var requestMock = HttpRequestMockUtility.CreateMockForGetRequest("details");
            // Setup IDurableEntityClient
            var durableClientMock = new Mock<IDurableEntityClient>();
            var entityId = new EntityId(nameof(CarEntity), plate);
            durableClientMock.Setup(c => c.ReadEntityStateAsync<JObject>(entityId, null, null))
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
        public async void Run_PlateExist_NoDetail_ReturnOk(string plate, CarData carData)
        {
            // Setup Ilogger
            var loggerMock = new Mock<ILogger<GetCarClient>>();
            //Setup HttpRequest            
            var requestMock = HttpRequestMockUtility.CreateMockForGetRequest(null);
            // Setup IDurableEntityClient
            var durableClientMock = new Mock<IDurableEntityClient>();
            var entityId = new EntityId(nameof(CarEntity), plate);
            durableClientMock.Setup(c => c.ReadEntityStateAsync<JObject>(entityId, null, null))
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
        public async void Run_PlateExist_WithDetail_ReturnOk(string plate, CarData carData, CarRentalsData carRentals)
        {
            // Setup Ilogger
            var loggerMock = new Mock<ILogger<GetCarClient>>();
            //Setup HttpRequest            
            var requestMock = HttpRequestMockUtility.CreateMockForGetRequest("details");
            // Setup IDurableEntityClient
            var durableClientMock = new Mock<IDurableEntityClient>();
            var entityId = new EntityId(nameof(CarEntity), plate);
            durableClientMock.Setup(c => c.ReadEntityStateAsync<JObject>(entityId, null, null))
                .Returns(() =>
                {
                    var entityState = JObject.FromObject(new { Status = carData });
                    return Task.FromResult(new EntityStateResponse<JObject>() { EntityState = entityState, EntityExists = true });
                });
            var rentalsEntityId = new EntityId(nameof(CarRentalsEntity), plate);
            durableClientMock.Setup(c => c.ReadEntityStateAsync<JObject>(rentalsEntityId, null, null))
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
