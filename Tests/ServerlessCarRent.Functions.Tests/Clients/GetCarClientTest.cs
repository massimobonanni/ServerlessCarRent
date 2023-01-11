using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Functions.Clients;
using ServerlessCarRent.Functions.Entities;
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
            var requestMock = new Mock<HttpRequest>();
            requestMock.SetupGet(r => r.Query)
                .Returns(() => new QueryCollection());
            // Setup IDurableEntityClient
            var durableClientMock = new Mock<IDurableEntityClient>();
            var entityId = new EntityId(nameof(CarEntity), plate);
            durableClientMock.Setup( c =>  c.ReadEntityStateAsync<JObject>(entityId, null, null))
                .Returns(() => Task.FromResult(new EntityStateResponse<JObject>() { EntityState = null, EntityExists = false }));

            // Create Class to test
            var funcClass = new GetCarClient(loggerMock.Object);


            // Call method under test
            var actual = await funcClass.Run(requestMock.Object, plate, durableClientMock.Object);

            // Assertions
            Assert.NotNull(actual);
            Assert.IsType<NotFoundResult>(actual);
        }
    }
}
