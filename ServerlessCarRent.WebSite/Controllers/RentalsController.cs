using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Functions.Requests;
using ServerlessCarRent.RestClient;
using ServerlessCarRent.WebSite.Models.RentalsController;
using ServerlessCarRent.WebSite.Services;
using ServerlessCarRent.WebSite.Utilities;
using System.Globalization;
using System.Reflection;

namespace ServerlessCarRent.WebSite.Controllers
{
    public class RentalsController : Controller
    {
        private readonly ILogger<RentalsController> _logger;
        private readonly RentalsManagementClient _rentalsManagementClient;
        private readonly CarsManagementClient _carsManagementClient;
        private readonly PickupLocationsManagementClient _pickupLocationsManagementClient;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public RentalsController(ILogger<RentalsController> logger,
            RentalsManagementClient rentalsManagementClient,
            CarsManagementClient carsManagementClient,
            PickupLocationsManagementClient pickupLocationsManagementClient,
            IConfiguration config, 
            IMapper mapper)
        {
            _logger = logger;
            _rentalsManagementClient = rentalsManagementClient;
            _carsManagementClient= carsManagementClient;
            _pickupLocationsManagementClient= pickupLocationsManagementClient;
            _config = config;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> Rent([FromQuery] string plate, [FromQuery] string location, 
            [FromQuery] string returnAction, [FromQuery] string returnController)
        {
            var car = await this._carsManagementClient.GetCarAsync(plate);

            if (car==null)
                return RedirectToAction(returnAction,returnController);

            var pickupLocation = await this._pickupLocationsManagementClient.GetPickupLocationAsync(location);
           
            if (pickupLocation == null)
                return RedirectToAction(returnAction, returnController);

            var viewModel = new RentViewModel();
            viewModel.ReturnAction = returnAction;
            viewModel.ReturnController = returnController;
            _mapper.Map(car, viewModel);
            _mapper.Map(pickupLocation, viewModel);
            viewModel.RentalStartDate = DateTimeOffset.Now;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Rent(RentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var rentalInfo = _mapper.Map<RentCarRequest>(viewModel);

                var result = await _rentalsManagementClient.RentCar(rentalInfo);

                if (result.Succeeded)
                    return RedirectToAction(viewModel.ReturnAction, viewModel.ReturnController);

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

           return View(viewModel);
        }

        [HttpGet]
        public async Task<ActionResult> Return([FromQuery] string plate, [FromQuery] string returnAction, [FromQuery] string returnController)
        {
            var car = await this._carsManagementClient.GetCarAsync(plate);

            if (car == null)
                return RedirectToAction(returnAction, returnController);

            if (car.CurrentRentalState!=Common.Models.CarRental.CarRentalState.Rented)
                return RedirectToAction(returnAction, returnController);

            var viewModel = new ReturnViewModel();
            viewModel.ReturnAction = returnAction;
            viewModel.ReturnController = returnController;
            _mapper.Map(car, viewModel);
            viewModel.RentalEndDate = DateTimeOffset.Now;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Return(ReturnViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var rentalInfo = _mapper.Map<ReturnCarRequest>(viewModel);

                var result = await _rentalsManagementClient.ReturnCar(viewModel.Plate,rentalInfo);

                if (result.Succeeded)
                    return RedirectToAction(viewModel.ReturnAction, viewModel.ReturnController);

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            return View(viewModel);
        }
    }
}
