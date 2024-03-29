﻿using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ServerlessCarRent.Common.Dtos;
using ServerlessCarRent.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerlessCarRent.Common.Models.PickupLocation;
using System.Text.Json.Serialization;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Common.Models.CarRental;
using ServerlessCarRent.Functions.Orchestrators;

namespace ServerlessCarRent.Functions.Entities
{
    public class CarEntity : ICarEntity
    {
        private readonly ILogger _logger;
        public CarEntity(ILogger logger)
        {
            _logger = logger;
        }

        [JsonPropertyName("status")]
        public CarData Status { get; set; }

        #region [ Public methods ]
        public void Initialize(InitializeCarDto carInfo)
        {
            if (Status == null)
                this.Status = new CarData();

            this.Status.Model = carInfo.Model;
            this.Status.PickupLocation = carInfo.PickupLocation;
            this.Status.CurrentState = carInfo.CarStatus;
            this.Status.CostPerHour = carInfo.CostPerHour;
            this.Status.Currency = carInfo.Currency;
            this.Status.CurrentRentalState = Common.Models.CarRental.CarRentalState.Free;

            if (!string.IsNullOrWhiteSpace(this.Status.PickupLocation))
            {
                SignalCarStatusChanged(this.Status.PickupLocation);
            }
        }

        public void Rent(RentCarDto rentInfo)
        {
            if (!this.Status.CanBeRent())
                return;

            var oldRentalStatus = this.Status.CurrentRentalState;

            this.Status.CurrentRentalState = Common.Models.CarRental.CarRentalState.Rented;
            this.Status.CurrentRental = new Common.Models.RentalData()
            {
                Id = rentInfo.RentalId,
                StartDate = rentInfo.StartDate
            };
            this.Status.CurrentRenter = new Common.Models.RenterData()
            {
                FirstName = rentInfo.RenterFirstName,
                LastName = rentInfo.RenterLastName,
                Email = rentInfo.RenterEmail
            };

            CallRentalStatusChangeOrchestrator(oldRentalStatus, this.Status.CurrentRentalState);

        }

        public Task<ReturnCarResponseDto> Return(ReturnCarDto returnInfo)
        {
            var response = new ReturnCarResponseDto()
            {
                Succeeded = false,
            };

            if (!this.Status.CanBeReturn())
                return Task.FromResult(response);

            var oldRentalStatus =
            this.Status.CurrentRental.EndDate = returnInfo.EndDate;

            response.RentalId = this.Status.CurrentRental.Id;
            response.CostPerHour = this.Status.CostPerHour;
            response.Currency = this.Status.Currency;
            response.Cost = this.Status.CalculateCost();
            response.Succeeded = true;

            SignalRentEnded();
            CallRentalStatusChangeOrchestrator(this.Status.CurrentRentalState, CarRentalState.Free);

            this.Status.CurrentRentalState = CarRentalState.Free;
            this.Status.CurrentRental = null;
            this.Status.CurrentRenter = null;

            SignalCarStatusChanged(this.Status.PickupLocation);

            return Task.FromResult(response);
        }

        public void Update(UpdateCarDto info)
        {
            if (!string.IsNullOrWhiteSpace(info.NewCurrency))
                this.Status.Currency = info.NewCurrency;

            if (!string.IsNullOrWhiteSpace(info.NewPickupLocation)
                && info.NewPickupLocation != this.Status.PickupLocation)
            {
                var oldPickupLocation = this.Status.PickupLocation;
                this.Status.PickupLocation = info.NewPickupLocation;
                SignalCarStatusChanged(oldPickupLocation);
                SignalCarStatusChanged(this.Status.PickupLocation);
            }

            if (info.NewCostPerHour.HasValue)
                this.Status.CostPerHour = info.NewCostPerHour.Value;

            if (info.NewCarStatus.HasValue
                && info.NewCarStatus.Value != this.Status.CurrentState)
            {
                this.Status.CurrentState = info.NewCarStatus.Value;
                SignalCarStatusChanged(this.Status.PickupLocation);
            }
        }

        public void Delete()
        {
            if (!this.Status.CanBeDeleted())
                return;
            DeleteRentals();
            Entity.Current.DeleteState();
        }
        #endregion [ Public methods ]

        #region [ Private methods ]
        private void DeleteRentals()
        {
            var carRentalsEntityId = new EntityId(nameof(CarRentalsEntity),
                             Entity.Current.EntityKey);

            Entity.Current.SignalEntity(carRentalsEntityId, "delete");
        }

        private void SignalCarStatusChanged(string pickupLocation)
        {
            var pickupLocationEntityId = new EntityId(nameof(PickupLocationEntity),
                                pickupLocation);

            Entity.Current.SignalEntity(pickupLocationEntityId,
                nameof(IPickupLocationEntity.CarStatusChanged),
                new CarStatusChangeDto()
                {
                    CarPlate = Entity.Current.EntityKey,
                    NewCarModel = this.Status.Model,
                    NewPickupLocation = this.Status.PickupLocation,
                    NewCarRentalStatus = this.Status.CurrentRentalState,
                    NewCarStatus = this.Status.CurrentState
                });
        }

        private void SignalRentEnded()
        {
            var carRentalsEntityId = new EntityId(nameof(CarRentalsEntity),
                            Entity.Current.EntityKey);

            Entity.Current.SignalEntity(carRentalsEntityId,
                nameof(ICarRentalsEntity.AddRent),
                new CarRentalDto()
                {
                    Cost = Status.CalculateCost(),
                    CostPerHour = this.Status.CostPerHour,
                    Currency = this.Status.Currency,
                    Renter = this.Status.CurrentRenter,
                    Rental = this.Status.CurrentRental
                });
        }

        private void CallRentalStatusChangeOrchestrator(CarRentalState oldRentalStatus, CarRentalState newRentalStatus)
        {
            var orchestratorDto = new RentalStatusChangeOrchestratorDto()
            {
                CarModel = this.Status.Model,
                CarPlate = Entity.Current.EntityKey,
                CarPickupLocation = this.Status.PickupLocation,
                Cost = newRentalStatus == CarRentalState.Rented ? null : Status.CalculateCost(),
                CostPerHour = this.Status.CostPerHour,
                Currency = this.Status.Currency,
                CurrentRental = this.Status.CurrentRental,
                CurrentRenter = this.Status.CurrentRenter,
                NewRentalStatus = newRentalStatus,
                OldRentalStatus = oldRentalStatus
            };

            Entity.Current.StartNewOrchestration(nameof(RentalStatusChangeOrchestrator),
                orchestratorDto);
        }

        #endregion [ Private methods ]

        [FunctionName(nameof(CarEntity))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx, ILogger logger)
            => ctx.DispatchAsync<CarEntity>(logger);

    }
}
