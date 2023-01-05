﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerlessCarRent.RestClient;
using ServerlessCarRent.WebSite.Models.PickupLocationsController;
using ServerlessCarRent.WebSite.Services;

namespace ServerlessCarRent.WebSite.Controllers
{
    public class PickupLocationsController : Controller
    {
        private readonly ILogger<PickupLocationsController> _logger;
        private readonly PickupLocationsManagementClient _pickupLocationsManagementClient;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly ICurrenciesService _currenciesService;

        public PickupLocationsController(ILogger<PickupLocationsController> logger,
            PickupLocationsManagementClient pickupLocationsManagementClient, IConfiguration config,
            IMapper mapper, ICurrenciesService currenciesService)
        {
            _logger = logger;
            _pickupLocationsManagementClient = pickupLocationsManagementClient;
            _config = config;
            _mapper = mapper;
            _currenciesService = currenciesService;
        }

        // GET: PickupLocationsController
        public async Task<ActionResult> Index([FromQuery] string identifierFilter, [FromQuery] string cityFilter,
            [FromQuery] string locationFilter)
        {
            var indexViewModel = new IndexViewModel();

            var searchResult = await this._pickupLocationsManagementClient.GetPickupLocationsAsync(identifierFilter, cityFilter, locationFilter, null);

            if (searchResult != null)
            {
                indexViewModel = _mapper.Map<IndexViewModel>(searchResult);
            }

            return View(indexViewModel);
        }

        // GET: PickupLocationsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PickupLocationsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PickupLocationsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PickupLocationsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PickupLocationsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PickupLocationsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PickupLocationsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
