using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ServerlessCarRent.Functions
{
    public class httptrigger
    {
        private readonly ILogger<httptrigger> _logger;

        public httptrigger(ILogger<httptrigger> logger)
        {
            _logger = logger;
        }

        [Function("httptrigger")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
