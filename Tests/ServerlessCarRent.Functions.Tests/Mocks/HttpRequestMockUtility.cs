using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;

namespace ServerlessCarRent.Functions.Tests.Mocks
{
    internal static class HttpRequestMockUtility
    {
        public static Mock<HttpRequest> CreateMockForGetRequest(string queryString)
        {
            var requestMock = new Mock<HttpRequest>();

            QueryCollection queryCollection = null;
            if (!string.IsNullOrWhiteSpace(queryString))
            {
                var queryDictionary = new Dictionary<string, StringValues>();
                var querySegments = queryString.Split('&');
                foreach (var segment in querySegments)
                {
                    var queryParts = segment.Split('=');
                    var key = queryParts[0];
                    var value = queryParts.Count() > 1 ? new StringValues(queryParts[1]) : StringValues.Empty;
                    queryDictionary.Add(key, value);
                }
                queryCollection= new QueryCollection(queryDictionary);
            }
            else
                queryCollection = new QueryCollection();

            requestMock.SetupGet(r => r.Query)
                .Returns(() => queryCollection);

            return requestMock;
        }
    }
}
