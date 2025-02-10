using FribergCarRentals.Models;
using FribergCarRentals.ViewModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Diagnostics;
using System.Security.Claims;
using FribergCarRentals.Data;

namespace FribergCarRentals.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRepository<Car> _carRepository;

        public HomeController(ILogger<HomeController> logger, IRepository<Car> carRepository)
        {
            _logger = logger;
            _carRepository = carRepository;
        }
        
        public async Task<IActionResult> Index()
        {
            var cars = await _carRepository.GetAllAsync();
            List<Car> latestEightCars = cars
                    .Where(car => car.IsAvailable)
                    .OrderByDescending(car => car.DateAdded)
                    .Take(8)
                    .ToList();

            return View(latestEightCars);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
