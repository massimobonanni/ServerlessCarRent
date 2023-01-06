using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.CarRental;
using ServerlessCarRent.Functions.Requests;
using ServerlessCarRent.Functions.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
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
            CancellationToken cancellationToken = default)
        {
            string query = string.Empty;
            if (!string.IsNullOrEmpty(plate))
                query += $"plate={plate}";
            if (!string.IsNullOrEmpty(location))
                query += $"location={location}";
            if (!string.IsNullOrEmpty(model))
                query += $"model={model}";
            if (carStates != null && carStates.Any())
                query += $"state={carStates.Select(s => s.ToString()).Aggregate((a, b) => $"{a}|{b}")}";
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
                var getResult = await response.Content.DeserializeObjectAsync<GetCarsResponse>();
                return getResult;
            }
            return null;
        }

        public async Task<GetCarResponse> GetCarAsync(string plate, CancellationToken cancellationToken = default)
        {
            var uri = this.CreateAPIUri(null, $"{DefaultApiEndpoint}/{plate}?details");

            var response = await this._httpClient.GetAsync(uri, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var car = await response.Content.DeserializeObjectAsync<GetCarResponse>();
                return car;
            }
            return null;
        }

        public async Task<ClientResult> CreateCarAsync(InitializeCarRequest car, CancellationToken cancellationToken = default)
        {
            if (car == null)
                throw new ArgumentNullException(nameof(car));

            var result = new ClientResult() { Succeeded = true };

            var uri = this.CreateAPIUri(null, $"{DefaultApiEndpoint}");

            var postContent = car.GenerateStringContent();

            var response = await this._httpClient.PostAsync(uri, postContent, cancellationToken);

            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.Created:
                    break;
                case System.Net.HttpStatusCode.Conflict:
                case System.Net.HttpStatusCode.BadRequest:
                    result.Succeeded = false;
                    var responseMessage = await response.Content.ReadAsStringAsync();
                    result.Errors.Add(responseMessage);
                    break;
            }

            return result;
        }

        public async Task<ClientResult> UpdateCarAsync(string carPlate, UpdateCarRequest carInfo, 
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(carPlate))
                throw new ArgumentNullException(nameof(carPlate));
            if (carInfo == null)
                throw new ArgumentNullException(nameof(carInfo));

            var result = new ClientResult() { Succeeded = true };

            var uri = this.CreateAPIUri(null, $"{DefaultApiEndpoint}/{carPlate}");

            var postContent = carInfo.GenerateStringContent();

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
