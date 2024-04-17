using Microsoft.DurableTask.Entities;
using Newtonsoft.Json.Linq;
using ServerlessCarRent.Functions.Entities;
using System.Threading;
using System.Threading.Tasks;
using Car = ServerlessCarRent.Common.Models.Car;
using CarRental = ServerlessCarRent.Common.Models.CarRental;
using PickupLocation = ServerlessCarRent.Common.Models.PickupLocation;

namespace Microsoft.DurableTask.Client.Entities
{
    /// <summary>
    /// Extension methods for <see cref="IDurableEntityClient"/>.
    /// </summary>
    internal static class DurableEntityClientExtensions
    {
        #region [ Generic Entity methods]

        public static async Task<EntityMetadata<JObject>> GetEntityStateAsync(this DurableEntityClient client,
           string entityName, string entityKey, CancellationToken token = default)
        {
            if (await client.EntityExistsAsync(entityName, entityKey, token))
            {
                var entityId = new EntityInstanceId(entityName, entityKey);
                var entity = await client.GetEntityAsync<JObject>(entityId, true, token);
                return entity;
            }
            return null;
        }

        public static async Task<bool> EntityExistsAsync(this DurableEntityClient client,
         string entityName, string entityKey, CancellationToken token = default)
        {
            var entity = await client.GetEntityStateAsync(entityName, entityKey, token);
            return entity != null;
        }

        public static async Task<TState> GetEntityStateAsync<TState>(this DurableEntityClient client,
          string entityName, string entityKey, CancellationToken token = default)
        {
            TState entityState = default;
            var entity = await client.GetEntityStateAsync(entityName, entityKey, token);
            if (entity != null)
            {
                var innerStatus = (JObject)entity.State.Property("State").Value;
                entityState = innerStatus.ToObject<TState>();
            }
            return entityState;
        }

        #endregion [ Generic Entity methods]

        #region [ Car Entity methods]

        public static Task<bool> CarExistsAsync(this DurableEntityClient client,
           string plate, CancellationToken token = default)
        {
            return client.EntityExistsAsync(nameof(CarEntity), plate, token);
        }

        public static Task<Car.CarData> GetCarDataAsync(this DurableEntityClient client,
            string plate, CancellationToken token = default)
        {
            return client.GetEntityStateAsync<Car.CarData>(nameof(CarEntity), plate, token);
        }

        #endregion [ Car Entity methods]

        #region [ Car Rentals Entity methods]

        public static Task<CarRental.CarRentalsData> GetCarRentalsDataAsync(this DurableEntityClient client,
            string plate, CancellationToken token = default)
        {
            return client.GetEntityStateAsync<CarRental.CarRentalsData>(nameof(CarRentalsEntity), plate, token);
        }

        #endregion [ Car Rentals Entity methods]

        #region [ PickupLocation Entity methods]

        public static Task<bool> PickupLocationExistsAsync(this DurableEntityClient client,
            string locationId, CancellationToken token = default)
        {
            return client.EntityExistsAsync(nameof(PickupLocationEntity), locationId, token);
        }

        public static Task<PickupLocation.PickupLocationData> GetPickupLocationDataAsync(this DurableEntityClient client,
            string locationId, CancellationToken token = default)
        {
            return client.GetEntityStateAsync<PickupLocation.PickupLocationData>(nameof(PickupLocationEntity), locationId, token);
        }

        #endregion [ PickupLocation Entity methods ]
    }
}
