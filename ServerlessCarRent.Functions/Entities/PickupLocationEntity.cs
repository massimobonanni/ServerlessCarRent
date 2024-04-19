using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Entities;
using Microsoft.Extensions.Logging;
using ServerlessCarRent.Common.Dtos;
using ServerlessCarRent.Common.Interfaces;
using ServerlessCarRent.Common.Models.PickupLocation;

namespace ServerlessCarRent.Functions.Entities
{
    public class PickupLocationEntity : EntityBase<PickupLocationData>,IPickupLocationEntity
	{
        public PickupLocationEntity(ILogger<PickupLocationEntity> logger):base(logger)
        {
            
        }

        #region [ Public methods ]
        public void Initialize(InitializePickupLocationDto locationInfo)
		{
			if (this.State == null)
				this.State = new PickupLocationData();

			this.State.Status = locationInfo.Status;
			this.State.City = locationInfo.City;
			this.State.Location = locationInfo.Location;
			this.State.Cars = new List<PickupLocationCarData>();
		}

		public Task<bool> RentCar(RentCarPickupLocationDto carInfo)
		{
			var car = this.State.Cars.FirstOrDefault(c => c.Plate == carInfo.CarPlate);

			if (car == null)
				return Task.FromResult(false);

			if (car.RentalStatus == Common.Models.CarRental.CarRentalState.Rented)
				return Task.FromResult(false); ;

			car.RentalStatus = Common.Models.CarRental.CarRentalState.Rented;

			SignalRentStarted(carInfo);

			return Task.FromResult(true);
		}

		public void CarStatusChanged(CarStatusChangeDto carInfo)
		{
			var car = this.State.Cars.FirstOrDefault(c => c.Plate == carInfo.CarPlate);

			if (!string.IsNullOrWhiteSpace(carInfo.NewPickupLocation)
				&& carInfo.NewPickupLocation != this.Context.Id.Key)
			{
				if (car != null)
					this.State.Cars.Remove(car);
				return;
			}

			if (car == null && carInfo.NewPickupLocation == this.Context.Id.Key)
			{
				car = new PickupLocationCarData()
				{
					Plate = carInfo.CarPlate,
					Model = carInfo.NewCarModel,
					RentalStatus = carInfo.NewCarRentalStatus.Value,
					Status = carInfo.NewCarStatus.Value
				};
				this.State.Cars.Add(car);
				return;
			}

			if (carInfo.NewCarStatus.HasValue
				&& carInfo.NewCarStatus.Value != car.Status)
				car.Status = carInfo.NewCarStatus.Value;

			if (carInfo.NewCarRentalStatus.HasValue
				&& carInfo.NewCarRentalStatus.Value != car.RentalStatus)
				car.RentalStatus = carInfo.NewCarRentalStatus.Value;
		}

        public void Update(UpdatePickupLocationDto info)
        {
			if (info == null)
				return;

			if (!string.IsNullOrWhiteSpace(info.Location))
				this.State.Location = info.Location;

			if (!string.IsNullOrWhiteSpace(info.City))
				this.State.City = info.City;

			this.State.Status = info.Status;

        }

        public void Delete()
        {
            if (!this.State.CanBeDeleted())
                return;
			this.State = null;
        }
        #endregion [ Public methods ]

        #region [ Private methods ]
        private void SignalRentStarted(RentCarPickupLocationDto carInfo)
		{
			var carRentalsEntityId = new EntityInstanceId(nameof(CarEntity), carInfo.CarPlate);

			this.Context.SignalEntity(carRentalsEntityId, nameof(ICarEntity.Rent),
				new RentCarDto()
				{
					RentalId=carInfo.RentalId,
					RenterFirstName=carInfo.RenterFirstName,
					RenterLastName=carInfo.RenterLastName,
					RenterEmail=carInfo.RenterEmail,
					StartDate=carInfo.RentalStart
				});
		}
		#endregion [ Private methods ]

		[Function(nameof(PickupLocationEntity))]
		public static Task Run([EntityTrigger] TaskEntityDispatcher ctx)
				=> ctx.DispatchAsync<PickupLocationEntity>();


    }
}
