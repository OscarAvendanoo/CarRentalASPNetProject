using FribergCarRentals.Data;
using FribergCarRentals.ViewModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FribergCarRentals.Models;
using Newtonsoft.Json.Linq;

namespace FribergCarRentals.Controllers
{
    public class CustomerController : Controller
    {
        private readonly CustomerRepository _CustomerRepository;
        private readonly IRepository<Car> _carRepository;
        private readonly IRepository<UserRole> _UserRoleRepository;
        public CustomerController(CustomerRepository customerRepository, IRepository<Car> carRepository, IRepository<UserRole> UserRoleRepository)
        {
            _CustomerRepository = customerRepository;
            _carRepository = carRepository;
            _UserRoleRepository = UserRoleRepository;
        }
        // GET: CustomerController
        public ActionResult Index()
        {
            return View();
        }
        public IActionResult LoginOrRegisterBeforeBooking(int carId)
        {
            // Hämtar bil och kollar så att ej är null
            var car = _carRepository.GetbyID(carId);
            if (car == null)
            {
                return NotFound();
            }
            // View model för att skicka car id till vyn, skapar även mallen för formuläret.
            var model = new StartBookingVM
            {
                LoginForm = new LoginAndBookVM { CarId = carId },
                RegisterForm = new RegisterBeforeBookingVM { CarId = carId }
            };

            if (User.Identity.IsAuthenticated)
            {
                // skapar ett anonymt objekt som parameter här, methoden som anropas behöver ett namn på värdet som skickas som parameter, carId vara både namnet här och i methodens parameter
                return RedirectToAction("ConfirmBooking", "Booking", new { carId = car.CarId });
            }

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LoginBeforeBook(LoginAndBookVM loginForm)
        {
            // validateAntiForgeryToken används för säkerhet, en token genereas i vyn som sedan skickas hit till post metoden för
            // att säkerställa att det är rätt formulär och användare som skickar datat. Förhindrar attacker där andra källor kan skapa postmetoder utifrån. 

            if (ModelState.IsValid)
            {
                // Store CarId temporarily for later use
                TempData["CarId"] = loginForm.CarId;

                var customer = _CustomerRepository.GetCustomerByEmail(loginForm.Email);

                if (customer != null && customer.Password == loginForm.Password)
                {
                    var claims = new List<Claim>{
                    new Claim(ClaimTypes.Name, customer.Email), // Use Email as the identifier
                    new Claim(ClaimTypes.Role, customer.Role.Role), // Store the user's role
                    new Claim("UserId", customer.CustomerId.ToString()) // Store custom properties if needed
                    };

                    // Skapar Identiteten med "claims", andra parametern är att vi ska använda denna identitet med cookie authentication system.
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // skapar authentication properties, detta göra så att cookien ligger kvar om browsern stängs om usern har klickat i "remember me"
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = loginForm.RememberMe
                    };

                    // Här loggas Usern in med dem claims och authentication properties som vi skapat
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                            new ClaimsPrincipal(claimsIdentity), authProperties);

                    // vid lyckad authenticering anropas home controllern och index metoden
                    return RedirectToAction("ConfirmBooking", "Booking", new { carId = loginForm.CarId });
                }
                else
                {
                    TempData["LoginError1"] = "The Email or Password was incorrect.";
                    return RedirectToAction("LoginOrRegisterBeforeBooking", "Customer", new { carId = loginForm.CarId });
                }
            }
            TempData["LoginError2"] = "You have to fill in all the fields and the correct format.";
            return RedirectToAction("LoginOrRegisterBeforeBooking", "Customer", new { carId = loginForm.CarId });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegisterBeforeBook(RegisterBeforeBookingVM RegisterForm)
        {
            if (ModelState.IsValid)
            {
                var customer = _CustomerRepository.GetCustomerByEmail(RegisterForm.Email);
                if (customer != null)
                {
                    TempData["ErrorRegister1"] = "There is already an account with that email address.";
                    return RedirectToAction("LoginOrRegisterBeforeBooking", "Customer", new { carId = RegisterForm.CarId });
                }

                var userRole = _UserRoleRepository.GetbyID(2);

                Customer newCustomer = new Customer
                {
                    FirstName = RegisterForm.FirstName,
                    LastName = RegisterForm.LastName,
                    Email = RegisterForm.Email,
                    Password = RegisterForm.Password,
                    Role = userRole
                };

                _CustomerRepository.Add(newCustomer);


                //TempData["CarId"] = RegisterForm.CarId;

                LoginAndBookVM loginForm = new LoginAndBookVM
                {
                    CarId = RegisterForm.CarId,
                    Email = RegisterForm.Email,
                    Password = RegisterForm.Password,
                    RememberMe = false
                };

                return View(loginForm);
            }
            else
            {
                TempData["ErrorRegister2"] = "There is already an account with that email address.";
            }
            return RedirectToAction("LoginOrRegisterBeforeBooking", "Customer", new { carId = RegisterForm.CarId });
        }
        // GET: CustomerController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CustomerController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CustomerController/Create
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

        // GET: CustomerController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CustomerController/Edit/5
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

        // GET: CustomerController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CustomerController/Delete/5
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
