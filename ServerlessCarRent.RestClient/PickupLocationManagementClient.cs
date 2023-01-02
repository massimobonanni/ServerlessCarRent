using Newtonsoft.Json;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.CarRental;
using ServerlessCarRent.Common.Models.PickupLocation;
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

		protected override string DefaultApiEndpoint => "api/pickuplocations";

		public async Task<GetPickupLocationsResponse> GetPickupLocationsAsync(string identifier, string city,string location, 
			IEnumerable<PickupLocationState> locationStates, CancellationToken cancellationToken=default)
		{
			string query = string.Empty;
			if (!string.IsNullOrEmpty(identifier))
				query += $"identifier={identifier}";
			if (!string.IsNullOrEmpty(city))
				query += $"city={city}";
			if (!string.IsNullOrEmpty(location))
				query += $"location={location}";
			if (locationStates != null && locationStates.Any())
				query += $"state={locationStates.Select(s=>s.ToString()).Aggregate((a,b)=> $"{a}|{b}")}";

			Uri uri;
			if (!string.IsNullOrEmpty(query))
				uri = this.CreateAPIUri($"{query}");
			else
				uri = this.CreateAPIUri();

			var response = await this._httpClient.GetAsync(uri, cancellationToken);

			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				var getResult = JsonConvert.DeserializeObject<GetPickupLocationsResponse>(content);
				return getResult;
			}
			return null;
		}
	}
}
