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
    public class RentalsManagementClient : RestClientBase
    {
        public RentalsManagementClient(HttpClient httpClient, string baseUrl, string apiKey) :
            base(httpClient, baseUrl, apiKey)
        {
        }

        protected override string DefaultApiEndpoint => "api/rents";

        public async Task<ClientResult> RentCar(RentCarRequest rentalInfo, 
            CancellationToken cancellationToken = default)
        {
            if (rentalInfo == null)
                throw new ArgumentNullException(nameof(rentalInfo));

            var result = new ClientResult() { Succeeded = true };

            var uri = this.CreateAPIUri(null, $"{DefaultApiEndpoint}");

            var postContent = rentalInfo.GenerateStringContent();

            var response = await this._httpClient.PostAsync(uri, postContent, cancellationToken);

            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
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

        public async Task<ClientResult> ReturnCar(string plate,
            ReturnCarRequest rentalInfo, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(plate))
                throw new ArgumentNullException(nameof(rentalInfo));
            if (rentalInfo == null)
                throw new ArgumentNullException(nameof(rentalInfo));

            var result = new ClientResult() { Succeeded = true };

            var uri = this.CreateAPIUri(null, $"{DefaultApiEndpoint}/{plate}");

            var postContent = rentalInfo.GenerateStringContent();

            var response = await this._httpClient.PutAsync(uri, postContent, cancellationToken);

            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
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
