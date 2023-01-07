using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerlessCarRent.Functions.Requests;
using ServerlessCarRent.RestClient;
using ServerlessCarRent.WebSite.Models.PickupLocationsController;
using ServerlessCarRent.WebSite.Services;
using ServerlessCarRent.WebSite.Utilities;

namespace ServerlessCarRent.WebSite.Controllers
{
    public class PickupLocationsController : Controller
    {
        private readonly ILogger<PickupLocationsController> _logger;
        private readonly PickupLocationsManagementClient _pickupLocationsManagementClient;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public PickupLocationsController(ILogger<PickupLocationsController> logger,
            PickupLocationsManagementClient pickupLocationsManagementClient, IConfiguration config,
            IMapper mapper)
        {
            _logger = logger;
            _pickupLocationsManagementClient = pickupLocationsManagementClient;
            _config = config;
            _mapper = mapper;
        }

        // GET: PickupLocationsController
        public async Task<ActionResult> Index(IndexViewModel viewModel)
        {
   
            var searchResult = await this._pickupLocationsManagementClient.GetPickupLocationsAsync(
                viewModel.IdentifierFilter,viewModel.CityFilter,viewModel.LocationFilter, viewModel.StatesFilter);

            if (searchResult != null)
            {
                 _mapper.Map(searchResult,viewModel);
            }

            return View(viewModel);
        }

        // GET: PickupLocationsController/Details/ROME-FCO
        public async Task<ActionResult> Details(string id)
        {
            var location = await this._pickupLocationsManagementClient.GetPickupLocationAsync(id);

            if (location == null)
                RedirectToAction("Index");

            var detailsViewModel = _mapper.Map<DetailsViewModel>(location);

            return View(detailsViewModel);
        }

        // GET: PickupLocationsController/Create
        public ActionResult Create()
        {
            var createViewModel = new CreateViewModel();

            createViewModel.PickupLocationStates = SelectListItemUtility.GenerateListFromPickupLocationStates(Common.Models.PickupLocation.PickupLocationState.Open);

            return View(createViewModel);
        }

        // POST: PickupLocationsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var location = _mapper.Map<InitializePickupLocationRequest>(viewModel);

                var result = await _pickupLocationsManagementClient.CreatePickupLocationAsync(location);
                if (result.Succeeded)
                    return RedirectToAction(nameof(Index));

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            viewModel.PickupLocationStates = SelectListItemUtility.GenerateListFromPickupLocationStates(Common.Models.PickupLocation.PickupLocationState.Open);

            return View(viewModel);
        }

        // GET: PickupLocationsController/Edit/ROME-FCO
        public async Task<ActionResult> Edit(string id)
        {
            var location = await this._pickupLocationsManagementClient.GetPickupLocationAsync(id);

            if (location == null)
                RedirectToAction("Index");

            var viewModel = _mapper.Map<EditViewModel>(location);

            viewModel.PickupLocationStates = SelectListItemUtility.GenerateListFromPickupLocationStates(viewModel.Status);

            return View(viewModel);
        }

        // POST: PickupLocationsController/Edit/ROME-FCO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, EditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var location = _mapper.Map<UpdatePickupLocationRequest>(viewModel);

                var result = await _pickupLocationsManagementClient.UpdatePickupLocationAsync(id, location);

                if (result.Succeeded)
                    return RedirectToAction(nameof(Index));

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            viewModel.PickupLocationStates = SelectListItemUtility.GenerateListFromPickupLocationStates(viewModel.Status);

            return View(viewModel);
        }

        
    }
}
