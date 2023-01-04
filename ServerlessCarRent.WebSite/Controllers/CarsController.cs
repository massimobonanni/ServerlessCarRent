using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServerlessCarRent.Functions.Requests;
using ServerlessCarRent.RestClient;
using ServerlessCarRent.WebSite.Models.CarsController;
using ServerlessCarRent.WebSite.Services;
using System.Reflection;

namespace ServerlessCarRent.WebSite.Controllers
{
    public class CarsController : Controller
    {
        private readonly ILogger<CarsController> _logger;
        private readonly CarsManagementClient _carsManagementClient;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly ICurrenciesService _currenciesService;

        public CarsController(ILogger<CarsController> logger,
            CarsManagementClient carsManagementClient, IConfiguration config, 
            IMapper mapper, ICurrenciesService currenciesService)
        {
            _logger = logger;
            _carsManagementClient = carsManagementClient;
            _config = config;
            _mapper = mapper;
            _currenciesService = currenciesService;
        }

        // GET: CarsController
        public async Task<ActionResult> Index([FromQuery] string plateFilter, [FromQuery] string locationFilter,
            [FromQuery] string modelFilter)
        {
            var indexViewModel = new IndexViewModel();

            var searchResult = await this._carsManagementClient.GetCarsAsync(plateFilter, locationFilter, modelFilter, null, null);

            if (searchResult != null)
            {
                indexViewModel = _mapper.Map<IndexViewModel>(searchResult);
            }

            return View(indexViewModel);
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
        public ActionResult Create()
        {
            var createViewModel = new CreateViewModel();

            createViewModel.Currencies = new List<SelectListItem>();
            createViewModel.Currencies.AddRange(_currenciesService.GetAll().Select(e => new SelectListItem(e,e)));
        
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

            viewModel.Currencies = new List<SelectListItem>();
            viewModel.Currencies.AddRange(_currenciesService.GetAll().Select(e => new SelectListItem(e, e)));

            return View(viewModel);
        }

        // GET: CarsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CarsController/Edit/5
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

        // GET: CarsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CarsController/Delete/5
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
