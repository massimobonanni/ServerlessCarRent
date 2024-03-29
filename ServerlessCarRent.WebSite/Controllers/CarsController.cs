﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServerlessCarRent.Common.Models.Car;
using ServerlessCarRent.Functions.Requests;
using ServerlessCarRent.RestClient;
using ServerlessCarRent.WebSite.Models.CarsController;
using ServerlessCarRent.WebSite.Services;
using ServerlessCarRent.WebSite.Utilities;
using System.Globalization;
using System.Reflection;

namespace ServerlessCarRent.WebSite.Controllers
{
    public class CarsController : Controller
    {
        private readonly ILogger<CarsController> _logger;
        private readonly CarsManagementClient _carsManagementClient;
        private readonly PickupLocationsManagementClient _pickupLocationsManagementClient;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly ICurrenciesService _currenciesService;

        public CarsController(ILogger<CarsController> logger,
            CarsManagementClient carsManagementClient,
            PickupLocationsManagementClient pickupLocationsManagementClient,
            IConfiguration config, 
            IMapper mapper, ICurrenciesService currenciesService)
        {
            _logger = logger;
            _carsManagementClient = carsManagementClient;
            _pickupLocationsManagementClient = pickupLocationsManagementClient;
            _config = config;
            _mapper = mapper;
            _currenciesService = currenciesService;
        }


        // GET: CarsController
        public async Task<ActionResult> Index([FromQuery] IndexViewModel viewModel)
        {
            var searchResult = await this._carsManagementClient.GetCarsAsync(viewModel.PlateFilter,
                viewModel.LocationFilter,viewModel.ModelFilter, viewModel.StatesFilter, viewModel.RentalStatesFilter);

            if (searchResult != null)
            {
                _mapper.Map(searchResult,viewModel);
            }

            return View(viewModel);
        }

        // GET: CarsController/Details/AA000BB
        public async Task<ActionResult> Details(string plate)
        {
            var car = await this._carsManagementClient.GetCarAsync(plate);

            if (car == null)
                RedirectToAction("Index");

            var detailsViewModel = _mapper.Map<DetailsViewModel>(car);

            return View(detailsViewModel);
        }

        // GET: CarsController/Create
        public async Task<ActionResult> Create()
        {
            var createViewModel = new CreateViewModel();

            createViewModel.Currencies = _currenciesService.GetAll().GenerateListItems();
            createViewModel.PickupLocations = 
                await SelectListItemUtility.GenerateListFromPickupLocationsAsync(_pickupLocationsManagementClient);

            return View(createViewModel);
        }

        // POST: CarsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var car = _mapper.Map<InitializeCarRequest>(viewModel);

                var result = await _carsManagementClient.CreateCarAsync(car);

                if (result.Succeeded)
                    return RedirectToAction(nameof(Index));

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            viewModel.Currencies = _currenciesService.GetAll().GenerateListItems();
            viewModel.PickupLocations =
                await SelectListItemUtility.GenerateListFromPickupLocationsAsync(_pickupLocationsManagementClient);

            return View(viewModel);
        }

        // GET: CarsController/Edit/AA000AA
        public async Task<ActionResult> Edit(string plate)
        {
            var car = await this._carsManagementClient.GetCarAsync(plate);

            if (car == null)
                RedirectToAction("Index");

            var viewModel = _mapper.Map<EditViewModel>(car);

            viewModel.Currencies = _currenciesService.GetAll().GenerateListItems();
            viewModel.CarStates = SelectListItemUtility.GenerateListFromCarStates(viewModel.CurrentState);
            viewModel.PickupLocations =
                await SelectListItemUtility.GenerateListFromPickupLocationsAsync(_pickupLocationsManagementClient);

            return View(viewModel);

        }

        // POST: CarsController/Edit/AA000AA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string plate, EditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var car = _mapper.Map<UpdateCarRequest>(viewModel);

                var result = await _carsManagementClient.UpdateCarAsync(plate,car);

                if (result.Succeeded)
                    return RedirectToAction(nameof(Index));

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            viewModel.Currencies = _currenciesService.GetAll().GenerateListItems();
            viewModel.CarStates = SelectListItemUtility.GenerateListFromCarStates(viewModel.CurrentState);
            viewModel.PickupLocations =
                await SelectListItemUtility.GenerateListFromPickupLocationsAsync(_pickupLocationsManagementClient);

            return View(viewModel);
        }

        
    }
}
