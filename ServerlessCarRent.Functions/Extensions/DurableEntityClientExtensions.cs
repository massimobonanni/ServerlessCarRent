using Newtonsoft.Json.Linq;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.CarRental;
using ServerlessCarRent.Functions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Extensions.DurableTask
{
	internal static class DurableEntityClientExtensions
	{
		#region [ Generic Entity methods]
		public static async Task<EntityStateResponse<JObject>> GetEntityStateAsync(this IDurableEntityClient client,
			string entityName, string entityKey, CancellationToken token = default)
		{
			var entityId = new EntityId(entityName, entityKey);
			EntityStateResponse<JObject> entity = await client.ReadEntityStateAsync<JObject>(entityId);
			return entity;
		}

		public static async Task<bool> EntityExistsAsync(this IDurableEntityClient client,
			string entityName, string entityKey, CancellationToken token = default)
		{
			var entity = await client.GetEntityStateAsync(entityName, entityKey, token);
			return entity.EntityExists;
		}

		public static async Task<TState> GetEntityStateAsync<TState>(this IDurableEntityClient client,
			string entityName, string entityKey, CancellationToken token = default)
		{
			TState entityState = default;
			var entity = await client.GetEntityStateAsync(entityName,entityKey, token);
			if (entity.EntityExists)
			{
				var innerStatus = (JObject)entity.EntityState.Property("Status").Value;
				entityState = innerStatus.ToObject<TState>();
			}
			return entityState;
		}
		#endregion [ Generic Entity methods]

		#region [ Car Entity methods]
		public static Task<bool> CarExistsAsync(this IDurableEntityClient client,
			string plate, CancellationToken token = default)
		{
			return client.EntityExistsAsync(nameof(CarEntity), plate, token);
		}

		public static Task<PickupLocationData> GetCarDataAsync(this IDurableEntityClient client,
			string plate, CancellationToken token = default)
		{
			return client.GetEntityStateAsync< PickupLocationData>(nameof(CarEntity), plate, token);

		}
		#endregion [ Car Entity methods]

		#region [ Car Rentals Entity methods]
		public static Task<CarRentalsData> GetCarRentalsDataAsync(this IDurableEntityClient client,
			string plate, CancellationToken token = default)
		{
			return client.GetEntityStateAsync<CarRentalsData>(nameof(CarRentalsEntity), plate, token);
		}
		#endregion [ Car Rentals Entity methods]

		#region [ PickupLocation Entity methods]
		public static Task<bool> PickupLocationExistsAsync(this IDurableEntityClient client,
			string locationId, CancellationToken token = default)
		{
			return client.EntityExistsAsync(nameof(PickupLocationEntity), locationId, token);
		}

		public static Task<PickupLocationData> GetPickupLocationDataAsync(this IDurableEntityClient client,
			string locationId, CancellationToken token = default)
		{
			return client.GetEntityStateAsync<PickupLocationData>(nameof(PickupLocationEntity), locationId, token);

		}
		#endregion [ PickupLocation Entity methods]
	}
}
