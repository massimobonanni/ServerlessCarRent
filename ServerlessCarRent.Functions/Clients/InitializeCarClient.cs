using Microsoft.AspNetCore.Http;
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

namespace ServerlessCarRent.Functions.Clients
{
	public class InitializeCarClient
	{
		private readonly ILogger _logger;

		public InitializeCarClient(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<InitializeCarClient>();
		}

		[OpenApiOperation(operationId: "initializeCar", new[] { "Cars Management" },
			Summary = "Create and initialize a new car", Visibility = OpenApiVisibilityType.Important)]
		[OpenApiRequestBody(contentType: "application/json", bodyType: typeof(InitializeCarRequest),
			Description = "Info about the car to create.", Required = true)]
		[OpenApiResponseWithBody(HttpStatusCode.Created, "application/json",
			typeof(InitializeCarResponse), Summary = "New car response.",
			Description = "If the request is valid, the response contains the info of the car created.")]
		[OpenApiResponseWithoutBody(HttpStatusCode.BadRequest,
			Summary = "The request is not valid because one of the plate or model is not valid")]
		[OpenApiResponseWithoutBody(HttpStatusCode.Conflict, Summary = "A car with the same plate already exists")]

		[FunctionName("InitializeCar")]
		public async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "post", Route = "cars")] HttpRequest req,
			[DurableClient] IDurableEntityClient client)
		{
			_logger.LogInformation("InitializeCar function");
			IActionResult responseData = null;

			try
			{
				string payloadContent = await new StreamReader(req.Body).ReadToEndAsync();
				var request = JsonConvert.DeserializeObject<InitializeCarRequest>(payloadContent);

				if (request != null && request.IsValid())
				{
					if (!await client.CarExistsAsync(request.Plate))
					{
						var entityId = new EntityId(nameof(CarEntity), request.Plate);
						var carDto = new InitializeCarDto()
						{
							Model = request.Model,
							PickupLocation = request.PickupLocation,
							CarStatus = request.CurrentStatus,
							CostPerHour = request.CostPerHour,
							Currency = request.Currency,
						};

						await client.SignalEntityAsync<ICarEntity>(entityId,
							e => e.Initialize(carDto));

						var response = new InitializeCarResponse()
						{
							Plate = request.Plate
						};

						responseData = new ObjectResult(response) { StatusCode = 201 };
					}
					else
					{
						responseData = new ConflictResult();
					}
				}
				else
				{
					responseData = new BadRequestResult();
				}
			}
			catch
			{
				responseData = new BadRequestResult();
			}

			return responseData;
		}
	}
}
