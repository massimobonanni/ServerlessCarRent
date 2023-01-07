using Newtonsoft.Json;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.CarRental;
using ServerlessCarRent.Common.Models.PickupLocation;
using ServerlessCarRent.Functions.Requests;
using ServerlessCarRent.Functions.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core.Tokens;

namespace ServerlessCarRent.RestClient
{
	public class PickupLocationsManagementClient : RestClientBase
	{
		public PickupLocationsManagementClient(HttpClient httpClient, string baseUrl, string apiKey) : 
			base(httpClient, baseUrl, apiKey)
		{
		}

		protected override string DefaultApiEndpoint => "api/pickuplocations";

		public async Task<GetPickupLocationsResponse> GetPickupLocationsAsync(string identifier, string city,string location, 
			IEnumerable<PickupLocationState> locationStates, CancellationToken cancellationToken=default)
		{
            var queryBuilder = new QueryStringBuilder();
            queryBuilder.Append("identifier", identifier);
            queryBuilder.Append("city", city);
            queryBuilder.Append("location", location);
            if (locationStates != null && locationStates.Any())
                queryBuilder.Append("state", locationStates.Select(s => s.ToString()).Aggregate((a, b) => $"{a}|{b}"));

            Uri uri;
            if (!queryBuilder.IsEmpty)
                uri = this.CreateAPIUri($"{queryBuilder}");
            else
                uri = this.CreateAPIUri();

            var response = await this._httpClient.GetAsync(uri, cancellationToken);

			if (response.IsSuccessStatusCode)
			{
				var getResult = await response.Content.DeserializeObjectAsync<GetPickupLocationsResponse>();
				return getResult;
			}
			return null;
		}

        public async Task<GetPickupLocationResponse> GetPickupLocationAsync(string identifier, CancellationToken cancellationToken = default)
        {
            var uri = this.CreateAPIUri(null, $"{DefaultApiEndpoint}/{identifier}?details");

            var response = await this._httpClient.GetAsync(uri, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var location = await response.Content.DeserializeObjectAsync<GetPickupLocationResponse>();
                return location;
            }
            return null;
        }

        public async Task<ClientResult> CreatePickupLocationAsync(InitializePickupLocationRequest pickupLocation, CancellationToken cancellationToken = default)
        {
            if (pickupLocation == null)
                throw new ArgumentNullException(nameof(pickupLocation));

            var result = new ClientResult() { Succeeded = true };

            var uri = this.CreateAPIUri(null, $"{DefaultApiEndpoint}");

            var postContent = pickupLocation.GenerateStringContent();

            var response = await this._httpClient.PostAsync(uri, postContent, cancellationToken);

            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.Created:
                    break;
                case System.Net.HttpStatusCode.Conflict:
                    result.Succeeded = false;
                    result.Errors.Add("A location with the same id already exists");
                    break;
                case System.Net.HttpStatusCode.BadRequest:
                    result.Succeeded = false;
                    var responseMessage = await response.Content.ReadAsStringAsync();
                    result.Errors.Add(responseMessage);
                    break;
            }

            return result;
        }

        public async Task<ClientResult> UpdatePickupLocationAsync(string id, UpdatePickupLocationRequest locationInfo,
             CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));
            if (locationInfo == null)
                throw new ArgumentNullException(nameof(locationInfo));

            var result = new ClientResult() { Succeeded = true };

            var uri = this.CreateAPIUri(null, $"{DefaultApiEndpoint}/{id}");

            var postContent = locationInfo.GenerateStringContent();

            var response = await this._httpClient.PutAsync(uri, postContent, cancellationToken);

            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.NoContent:
                    break;
                case System.Net.HttpStatusCode.NotFound:
                case System.Net.HttpStatusCode.BadRequest:
                    result.Succeeded = false;
                    var responseMessage = await response.Content.ReadAsStringAsync();
                    result.Errors.Add(responseMessage);
                    break;
            }

            return result;
        }
    }
}
