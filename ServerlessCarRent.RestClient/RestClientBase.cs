using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.RestClient
{
	public abstract class RestClientBase
	{
		protected virtual string DefaultApiEndpoint { get; }

		protected readonly HttpClient _httpClient;
		protected readonly string _baseUrl;
		protected readonly string _apiKey;

		public RestClientBase(HttpClient httpClient, string baseUrl, string apiKey)
		{
			this._httpClient = httpClient;
			if (baseUrl.EndsWith("/"))
				baseUrl = baseUrl.Remove(baseUrl.Length - 1, 1);
			this._baseUrl = baseUrl;
			this._apiKey = apiKey;
		}

		protected virtual string GetFullUrl()
		{
			return $"{this._baseUrl}/{this.DefaultApiEndpoint}";
		}


		protected virtual Uri CreateAPIUri(string queryString = null, string overrideApiEndpoint = null)
		{
			string url = this.GetFullUrl();

			if (!string.IsNullOrEmpty(overrideApiEndpoint))
				url = $"{this._baseUrl}/{overrideApiEndpoint}";

			if (!string.IsNullOrWhiteSpace(queryString))
			{
				if (queryString.StartsWith("?"))
					queryString = queryString.Remove(0, 1);
				url = $"{url}?{queryString}";
			}

			if (!string.IsNullOrWhiteSpace(this._apiKey))
			{
				if (!url.Contains("?"))
				{
					url = $"{url}?code={this._apiKey}";
				}
				else
				{
					url = $"{url}&code={this._apiKey}";
				}
			}

			return new Uri(url);
		}

	}
}
