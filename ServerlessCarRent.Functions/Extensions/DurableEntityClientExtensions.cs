using Microsoft.DurableTask.Entities;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.CarRental;
using ServerlessCarRent.Common.Models.PickupLocation;
using ServerlessCarRent.Functions.Entities;

namespace Microsoft.DurableTask.Client.Entities
{
    /// <summary>
    /// Extension methods for <see cref="IDurableEntityClient"/>.
    /// </summary>
    internal static class DurableEntityClientExtensions
    {
        #region [ Generic Entity methods]

        public static async Task<EntityMetadata> GetEntityAsync(this DurableEntityClient client,
           string entityName, string entityKey, CancellationToken token = default)
        {
            var entityId = new EntityInstanceId(entityName, entityKey);
            var entity = await client.GetEntityAsync(entityId, true, token);
            return entity;
        }

        public static async Task<bool> EntityExistsAsync(this DurableEntityClient client,
         string entityName, string entityKey, CancellationToken token = default) 
        {
            var entity = await client.GetEntityAsync(entityName, entityKey, token);
            return entity != null;
        }

        public static async Task<TState> GetEntityStateAsync<TState>(this DurableEntityClient client,
          string entityName, string entityKey, CancellationToken token = default) 
        {
            TState entityState = default;
            var entity = await client.GetEntityAsync(entityName, entityKey, token);
            if (entity != null)
            {
                entityState = entity.State.ReadAs<TState>();
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

        public static Task<CarData> GetCarDataAsync(this DurableEntityClient client,
            string plate, CancellationToken token = default)
        {
            return client.GetEntityStateAsync<CarData>(nameof(CarEntity), plate, token);
        }

        #endregion [ Car Entity methods]

        #region [ Car Rentals Entity methods]

        public static Task<CarRentalsData> GetCarRentalsDataAsync(this DurableEntityClient client,
            string plate, CancellationToken token = default)
        {
            return client.GetEntityStateAsync<CarRentalsData>(nameof(CarRentalsEntity), plate, token);
        }

        #endregion [ Car Rentals Entity methods]

        #region [ PickupLocation Entity methods]

        public static Task<bool> PickupLocationExistsAsync(this DurableEntityClient client,
            string locationId, CancellationToken token = default)
        {
            return client.EntityExistsAsync(nameof(PickupLocationEntity), locationId, token);
        }

        public static Task<PickupLocationData> GetPickupLocationDataAsync(this DurableEntityClient client,
            string locationId, CancellationToken token = default)
        {
            return client.GetEntityStateAsync<PickupLocationData>(nameof(PickupLocationEntity), locationId, token);
        }

        #endregion [ PickupLocation Entity methods ]
    }
}
