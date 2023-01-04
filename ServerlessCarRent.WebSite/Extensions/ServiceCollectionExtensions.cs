using ServerlessCarRent.RestClient;
using ServerlessCarRent.WebSite.Services;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{
		public static void AddManagementClients(this IServiceCollection services)
		{
			services.AddScoped<CarsManagementClient>(s =>
			{
				var config = s.GetService<IConfiguration>();
				var httpClient = s.GetService<HttpClient>();

				var baseUrl = config.GetValue<string>("APISettings:BaseUrl");
				var apiKey = config.GetValue<string>("APISettings:ApiKey");

				var client = new CarsManagementClient(httpClient, baseUrl, apiKey);

				return client;
			});

			services.AddScoped<PickupLocationsManagementClient>(s =>
			{
				var config = s.GetService<IConfiguration>();
				var httpClient = s.GetService<HttpClient>();

				var baseUrl = config.GetValue<string>("APISettings:BaseUrl");
				var apiKey = config.GetValue<string>("APISettings:ApiKey");

				var client = new PickupLocationsManagementClient(httpClient, baseUrl, apiKey);

				return client;
			});

			services.AddScoped<ICurrenciesService, CurrenciesService>();
		}
	}
}
