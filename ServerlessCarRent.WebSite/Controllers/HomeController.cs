using Microsoft.AspNetCore.Mvc;
using ServerlessCarRent.RestClient;
using ServerlessCarRent.WebSite.Models;
using System.Diagnostics;

namespace ServerlessCarRent.WebSite.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly CarsManagementClient _carsManagementClient;
		private readonly IConfiguration _config;

		public HomeController(ILogger<HomeController> logger, 
			CarsManagementClient carsManagementClient, IConfiguration config)
		{
			_logger = logger;
			_carsManagementClient=carsManagementClient;
			_config = config;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}