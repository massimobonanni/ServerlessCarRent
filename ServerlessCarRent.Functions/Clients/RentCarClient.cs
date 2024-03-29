﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServerlessCarRent.Functions.Requests;
using ServerlessCarRent.Functions.Responses;
using ServerlessCarRent.Functions.Entities;
using ServerlessCarRent.Common.Dtos;
using ServerlessCarRent.Common.Interfaces;
using ServerlessCarRent.Functions.Orchestrators;

namespace ServerlessCarRent.Functions.Clients
{
    public class RentCarClient
    {
        private readonly ILogger<RentCarClient> _logger;

        public RentCarClient(ILogger<RentCarClient> logger)
        {
            _logger = logger;
        }

        [OpenApiOperation(operationId: "rentCar", new[] { "Rentals Management" },
            Summary = "Rent a car", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(RentCarRequest),
            Description = "Info about the rent.", Required = true)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json",
            typeof(RentCarResponse), Summary = "Rent operation response.",
            Description = "If the request is valid, the response contains the info of the rent.")]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, "text/plain", typeof(string),
            Summary = "The request is not valid. The API returns a message with the error")]
        [OpenApiResponseWithoutBody(HttpStatusCode.NotFound,
            Summary = "The car is not exists")]

        [FunctionName("RentCar")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "rents")] HttpRequest req,
            [DurableClient] IDurableClient client)
        {
            _logger.LogInformation("RentCar function");

            try
            {
                string payloadContent = await new StreamReader(req.Body).ReadToEndAsync();
                var request = JsonConvert.DeserializeObject<RentCarRequest>(payloadContent);

                if (request == null || !request.IsValid())
                    return new BadRequestObjectResult("The rent car info are not valid");

                var car = await client.GetCarDataAsync(request.CarPlate);

                if (car == null)
                    return new NotFoundObjectResult("The car is not exist");

                if (!car.CanBeRent())
                    new BadRequestObjectResult("The car cannot be rented");

                var location = await client.GetPickupLocationDataAsync(car.PickupLocation);

                if (location.Status != Common.Models.PickupLocation.PickupLocationState.Open)
                    return new BadRequestObjectResult("The pickup location is not open");

                if (car.PickupLocation != request.PickupLocation)
                    return new BadRequestObjectResult("The car location is not the same of the rent location"); ;

                var orchestrationDto = new RentOrchestratorDto()
                {
                    CarPlate = request.CarPlate,
                    PickupLocation = request.PickupLocation,
                    RentalStartDate = request.RentalStartDate,
                    RenterFirstName = request.RenterFirstName,
                    RenterLastName = request.RenterLastName,
                    RenterEmail = request.RenterEmail,
                };

                var rentOperationId = await client.StartNewAsync(nameof(RentOrchestrator), orchestrationDto);

                await client.WaitForCompletionOrCreateCheckStatusResponseAsync(req, rentOperationId,
                    TimeSpan.FromSeconds(10));

                var response = new RentCarResponse()
                {
                    CarPlate = request.CarPlate,
                    PickupLocation = request.PickupLocation,
                    RentalId = rentOperationId,
                    RentOperationStatus = RentOperationState.Pending
                };

                var orchestratorStatus = await client.GetStatusAsync(rentOperationId);
                if (orchestratorStatus.RuntimeStatus == OrchestrationRuntimeStatus.Completed)
                {
                    var orchestratorOutput = orchestratorStatus.Output.ToObject<RentOrchestratorResponseDto>();
                    response.RentOperationStatus = orchestratorOutput.Status;
                }

                return new OkObjectResult(response);

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "RentCar");
                throw;
            }
        }
    }
}
