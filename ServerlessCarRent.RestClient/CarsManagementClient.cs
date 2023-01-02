using Newtonsoft.Json;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.CarRental;
using ServerlessCarRent.Functions.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core.Tokens;

namespace ServerlessCarRent.RestClient
{
	public class CarsManagementClient : RestClientBase
	{
		public CarsManagementClient(HttpClient httpClient, string baseUrl, string apiKey) : 
			base(httpClient, baseUrl, apiKey)
		{
		}

		protected override string DefaultApiEndpoint => "api/cars";

		public async Task<GetCarsResponse> GetCarsAsync(string plate, string location, string model, 
			IEnumerable<CarState> carStates, IEnumerable<CarRentalState> carRentalStates, 
			CancellationToken cancellationToken=default)
		{
			string query = string.Empty;
			if (!string.IsNullOrEmpty(plate))
				query += $"plate={plate}";
			if (!string.IsNullOrEmpty(location))
				query += $"location={location}";
			if (!string.IsNullOrEmpty(model))
				query += $"model={model}";
			if (carStates!=null && carStates.Any())
				query += $"state={carStates.Select(s=>s.ToString()).Aggregate((a,b)=> $"{a}|{b}")}";
			if (carRentalStates != null && carRentalStates.Any())
				query += $"rentalState={carRentalStates.Select(s => s.ToString()).Aggregate((a, b) => $"{a}|{b}")}";

			Uri uri;
			if (!string.IsNullOrEmpty(query))
				uri = this.CreateAPIUri($"{query}");
			else
				uri = this.CreateAPIUri();

			var response = await this._httpClient.GetAsync(uri, cancellationToken);

			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				var getResult = JsonConvert.DeserializeObject<GetCarsResponse>(content);
				return getResult;
			}
			return null;
		}
	}
}
