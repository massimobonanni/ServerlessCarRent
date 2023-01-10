using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using ServerlessCarRent.Common.Dtos;
using ServerlessCarRent.Common.Interfaces;
using ServerlessCarRent.Common.Models.PickupLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace ServerlessCarRent.Functions.Entities
{
	public class PickupLocationEntity : IPickupLocationEntity
	{
		private readonly ILogger _logger;
		public PickupLocationEntity(ILogger logger)
		{
			_logger = logger;
		}

		[JsonPropertyName("status")]
		public PickupLocationData Status { get; set; }

		#region [ Public methods ]
		public void Initialize(InitializePickupLocationDto locationInfo)
		{
			if (this.Status == null)
				this.Status = new PickupLocationData();

			this.Status.Status = locationInfo.Status;
			this.Status.City = locationInfo.City;
			this.Status.Location = locationInfo.Location;
			this.Status.Cars = new List<PickupLocationCarData>();
		}

		public Task<bool> RentCar(RentCarPickupLocationDto carInfo)
		{
			var car = this.Status.Cars.FirstOrDefault(c => c.Plate == carInfo.CarPlate);

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
			var car = this.Status.Cars.FirstOrDefault(c => c.Plate == carInfo.CarPlate);

			if (!string.IsNullOrWhiteSpace(carInfo.NewPickupLocation)
				&& carInfo.NewPickupLocation != Entity.Current.EntityKey)
			{
				if (car != null)
					this.Status.Cars.Remove(car);
				return;
			}

			if (car == null && carInfo.NewPickupLocation == Entity.Current.EntityKey)
			{
				car = new PickupLocationCarData()
				{
					Plate = carInfo.CarPlate,
					Model = carInfo.NewCarModel,
					RentalStatus = carInfo.NewCarRentalStatus.Value,
					Status = carInfo.NewCarStatus.Value
				};
				this.Status.Cars.Add(car);
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
				this.Status.Location = info.Location;

			if (!string.IsNullOrWhiteSpace(info.City))
				this.Status.City = info.City;

			this.Status.Status = info.Status;

        }

        public void Delete()
        {
            if (!this.Status.CanBeDeleted())
                return;
            Entity.Current.DeleteState();
        }
        #endregion [ Public methods ]

        #region [ Private methods ]
        private void SignalRentStarted(RentCarPickupLocationDto carInfo)
		{
			var carRentalsEntityId = new EntityId(nameof(CarEntity), carInfo.CarPlate);

			Entity.Current.SignalEntity(carRentalsEntityId,	nameof(ICarEntity.Rent),
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

		[FunctionName(nameof(PickupLocationEntity))]
		public static Task Run([EntityTrigger] IDurableEntityContext ctx, ILogger logger)
				=> ctx.DispatchAsync<PickupLocationEntity>(logger);


    }
}
