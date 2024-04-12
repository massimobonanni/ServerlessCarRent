using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using ServerlessCarRent.Functions.Clients;
using ServerlessCarRent.Functions.Entities;
using ServerlessCarRent.Functions.Responses;
using ServerlessCarRent.Functions.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.Functions.Tests.Clients
{
    public class GetCarsClientTest
    {
        //[Fact]
        //public async void Run_ShouldReturnEmptyList_WhenNoCarExist()
        //{
        //    // Setup Ilogger
        //    var loggerMock = new Mock<ILogger<GetCarsClient>>();
        //    // Setup HttpRequest            
        //    var requestMock = HttpRequestMockUtility.CreateMockForGetRequest(null);
        //    // Setup EntityQueryResult
        //    var entityQueryResultMock = new Mock<EntityQueryResult>();
        //    entityQueryResultMock.SetupGet(e => e.Entities).Returns((IEnumerable<DurableEntityStatus>)null);
        //    entityQueryResultMock.SetupGet(e => e.ContinuationToken).Returns((string)null);
        //    // Setup IDurableEntityClient
        //    var durableClientMock = new Mock<IDurableEntityClient>();
        //    durableClientMock.Setup(c => c.ListEntitiesAsync(It.IsAny<EntityQuery>(), default))
        //        .Returns(() => Task.FromResult(entityQueryResultMock.Object));

        //    // Create Class to test
        //    var funcClass = new GetCarsClient(loggerMock.Object);

        //    // Call method under test
        //    var actual = await funcClass.Run(requestMock.Object, durableClientMock.Object);

        //    // Assertions
        //    Assert.NotNull(actual);
        //    Assert.IsType<OkObjectResult>(actual);
        //    var response = ((OkObjectResult)actual).Value;
        //    Assert.IsType<GetCarsResponse>(response);
        //    var getCarResponse= (GetCarsResponse)response;
        //    Assert.Empty(getCarResponse.Cars);
        //}

    }
}
