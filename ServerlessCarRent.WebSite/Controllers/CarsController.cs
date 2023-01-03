using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerlessCarRent.RestClient;
using ServerlessCarRent.WebSite.Models.CarsController;

namespace ServerlessCarRent.WebSite.Controllers
{
    public class CarsController : Controller
    {
        private readonly ILogger<CarsController> _logger;
        private readonly CarsManagementClient _carsManagementClient;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public CarsController(ILogger<CarsController> logger,
            CarsManagementClient carsManagementClient, IConfiguration config, IMapper mapper)
        {
            _logger = logger;
            _carsManagementClient = carsManagementClient;
            _config = config;
            _mapper = mapper;
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
            return View();
        }

        // POST: CarsController/Create
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
