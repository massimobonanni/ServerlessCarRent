using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.DurableTask.Entities;
using Microsoft.Extensions.Logging;
using ServerlessCarRent.Common.Dtos;
using ServerlessCarRent.Common.Interfaces;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.CarRental;
using ServerlessCarRent.Functions.Orchestrators;
using System.Threading.Tasks;
using EntityTriggerAttribute = Microsoft.Azure.Functions.Worker.EntityTriggerAttribute;

namespace ServerlessCarRent.Functions.Entities
{
    public class CarEntity : TaskEntity<CarData>, ICarEntity
    {
        private readonly ILogger _logger;
        public CarEntity(ILogger logger)
        {
            _logger = logger;
        }

        #region [ Public methods ]
        public void Initialize(InitializeCarDto carInfo)
        {
            if (this.State == null)
                this.State = new CarData();

            this.State.Model = carInfo.Model;
            this.State.PickupLocation = carInfo.PickupLocation;
            this.State.CurrentState = carInfo.CarStatus;
            this.State.CostPerHour = carInfo.CostPerHour;
            this.State.Currency = carInfo.Currency;
            this.State.CurrentRentalState = Common.Models.CarRental.CarRentalState.Free;

            if (!string.IsNullOrWhiteSpace(this.State.PickupLocation))
            {
                SignalCarStatusChanged(this.State.PickupLocation);
            }
        }

        public void Rent(RentCarDto rentInfo)
        {
            if (!this.State.CanBeRent())
                return;

            var oldRentalStatus = this.State.CurrentRentalState;

            this.State.CurrentRentalState = CarRentalState.Rented;
            this.State.CurrentRental = new Common.Models.RentalData()
            {
                Id = rentInfo.RentalId,
                StartDate = rentInfo.StartDate
            };
            this.State.CurrentRenter = new Common.Models.RenterData()
            {
                FirstName = rentInfo.RenterFirstName,
                LastName = rentInfo.RenterLastName,
                Email = rentInfo.RenterEmail
            };

            CallRentalStatusChangeOrchestrator(oldRentalStatus, 
                this.State.CurrentRentalState);

        }

        public Task<ReturnCarResponseDto> Return(ReturnCarDto returnInfo)
        {
            var response = new ReturnCarResponseDto()
            {
                Succeeded = false,
            };

            if (!this.State.CanBeReturn())
                return Task.FromResult(response);

            var oldRentalStatus =
            this.State.CurrentRental.EndDate = returnInfo.EndDate;

            response.RentalId = this.State.CurrentRental.Id;
            response.CostPerHour = this.State.CostPerHour;
            response.Currency = this.State.Currency;
            response.Cost = this.State.CalculateCost();
            response.Succeeded = true;

            SignalRentEnded();
            CallRentalStatusChangeOrchestrator(this.State.CurrentRentalState, CarRentalState.Free);

            this.State.CurrentRentalState = CarRentalState.Free;
            this.State.CurrentRental = null;
            this.State.CurrentRenter = null;

            SignalCarStatusChanged(this.State.PickupLocation);

            return Task.FromResult(response);
        }

        public void Update(UpdateCarDto info)
        {
            if (!string.IsNullOrWhiteSpace(info.NewCurrency))
                this.State.Currency = info.NewCurrency;

            if (!string.IsNullOrWhiteSpace(info.NewPickupLocation)
                && info.NewPickupLocation != this.State.PickupLocation)
            {
                var oldPickupLocation = this.State.PickupLocation;
                this.State.PickupLocation = info.NewPickupLocation;
                SignalCarStatusChanged(oldPickupLocation);
                SignalCarStatusChanged(this.State.PickupLocation);
            }

            if (info.NewCostPerHour.HasValue)
                this.State.CostPerHour = info.NewCostPerHour.Value;

            if (info.NewCarStatus.HasValue
                && info.NewCarStatus.Value != this.State.CurrentState)
            {
                this.State.CurrentState = info.NewCarStatus.Value;
                SignalCarStatusChanged(this.State.PickupLocation);
            }
        }

        public void Delete()
        {
            if (!this.State.CanBeDeleted())
                return;
            DeleteRentals();
            this.State = null;
        }
        #endregion [ Public methods ]

        #region [ Private methods ]
        private void DeleteRentals()
        {
            var carRentalsEntityId = new EntityInstanceId(nameof(CarRentalsEntity),
                             this.Context.Id.Key);

            this.Context.SignalEntity(carRentalsEntityId, "delete");
        }

        private void SignalCarStatusChanged(string pickupLocation)
        {
            var pickupLocationEntityId = new EntityInstanceId(nameof(PickupLocationEntity),
                                pickupLocation);

            this.Context.SignalEntity(pickupLocationEntityId,
                nameof(IPickupLocationEntity.CarStatusChanged),
                new CarStatusChangeDto()
                {
                    CarPlate = this.Context.Id.Key,
                    NewCarModel = this.State.Model,
                    NewPickupLocation = this.State.PickupLocation,
                    NewCarRentalStatus = this.State.CurrentRentalState,
                    NewCarStatus = this.State.CurrentState
                });
        }

        private void SignalRentEnded()
        {
            var carRentalsEntityId = new EntityInstanceId(nameof(CarRentalsEntity),
                            this.Context.Id.Key);

            this.Context.SignalEntity(carRentalsEntityId,
                nameof(ICarRentalsEntity.AddRent),
                new CarRentalDto()
                {
                    Cost = State.CalculateCost(),
                    CostPerHour = this.State.CostPerHour,
                    Currency = this.State.Currency,
                    Renter = this.State.CurrentRenter,
                    Rental = this.State.CurrentRental
                });
        }

        private void CallRentalStatusChangeOrchestrator(CarRentalState oldRentalStatus, CarRentalState newRentalStatus)
        {
            var orchestratorDto = new RentalStatusChangeOrchestratorDto()
            {
                CarModel = this.State.Model,
                CarPlate = this.Context.Id.Key,
                CarPickupLocation = this.State.PickupLocation,
                Cost = newRentalStatus == CarRentalState.Rented ? null : State.CalculateCost(),
                CostPerHour = this.State.CostPerHour,
                Currency = this.State.Currency,
                CurrentRental = this.State.CurrentRental,
                CurrentRenter = this.State.CurrentRenter,
                NewRentalStatus = newRentalStatus,
                OldRentalStatus = oldRentalStatus
            };

            this.Context.ScheduleNewOrchestration(nameof(RentalStatusChangeOrchestrator),
                orchestratorDto);
        }

        #endregion [ Private methods ]

        [FunctionName(nameof(CarEntity))]
        public static Task Run([EntityTrigger] TaskEntityDispatcher ctx)
            => ctx.DispatchAsync<CarEntity>();

    }
}
