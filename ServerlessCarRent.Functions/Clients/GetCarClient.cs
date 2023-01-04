using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServerlessCarRent.Functions.Responses;

namespace ServerlessCarRent.Functions.Clients
{
	public class GetCarClient
	{
		private readonly ILogger _logger;

		public GetCarClient(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<GetCarClient>();
		}

		[OpenApiOperation(operationId: "getCar", new[] { "Cars Management" },
		   Summary = "Retrieve the information about a car", Visibility = OpenApiVisibilityType.Important)]
		[OpenApiParameter("plate", Summary = "The plate of the car to retrieve",
		   In = Microsoft.OpenApi.Models.ParameterLocation.Path, Required = true,
		   Visibility = OpenApiVisibilityType.Important)]
		[OpenApiParameter("details", Summary = "Set this parameter if you also want rentals details",
		   In = Microsoft.OpenApi.Models.ParameterLocation.Query, Required = false,
		   Visibility = OpenApiVisibilityType.Important)]
		[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json",
			typeof(GetCarResponse), Summary = "The car details",
			Description = "If the car exists, the response contains the detail of the car.")]
		[OpenApiResponseWithoutBody(HttpStatusCode.NotFound,
		   Summary = "The car with the search plate doesn't exist")]

		[FunctionName("GetCar")]
		public async Task<IActionResult> Run(
		   [HttpTrigger(AuthorizationLevel.Function, "get", Route = "cars/{plate}")] HttpRequest req,
		   string plate,
		   [DurableClient] IDurableEntityClient client)
		{
			_logger.LogInformation("GetCar function");
			IActionResult responseData = null;

			var details = req.Query.Any(e => e.Key.ToLower() == "details");
			try
			{
				var carData = await client.GetCarDataAsync(plate);
				if (carData != null)
				{
					var response = new GetCarResponse()
					{
						Plate = plate,
						Model = carData.Model,
						PickupLocation = carData.PickupLocation,
						CurrentRentalState = carData.CurrentRentalState,
						CurrentState = carData.CurrentState,
						Currency= carData.Currency,
						CostPerHour= carData.CostPerHour,
						CurrentRenterEmail=carData.CurrentRenter?.Email,
						CurrentRenterFirstName=carData.CurrentRenter?.FirstName,
						CurrentRenterLastName=carData.CurrentRenter?.LastName,
					};

					if (details)
					{
						var carRentalsData = await client.GetCarRentalsDataAsync(plate);
						if (carRentalsData!= null)
						{
							response.Rentals = carRentalsData.Rentals
								.Select(r =>
									new GetCarResponse.CarRentalDto()
									{
										RentalId=r.Rental.Id,
										CostPerHour=r.CostPerHour,
										Currency=r.Currency,
										RentalStart=r.Rental.StartDate,
										RentalEnd=r.Rental.EndDate,
										RenterFirstName=r.Renter.FirstName,
										RenterLastName=r.Renter.LastName,
										RenterEmail=r.Renter.Email,
										Cost=r.TotalCost
									})
								.ToList(); 
						}
					}

					responseData = new OkObjectResult(response);
				}
				else
				{
					responseData = new NotFoundResult();
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

