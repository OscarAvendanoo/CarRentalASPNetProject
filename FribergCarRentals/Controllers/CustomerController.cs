using FribergCarRentals.Data;
using FribergCarRentals.ViewModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FribergCarRentals.Models;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.SqlServer.Server;

namespace FribergCarRentals.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _CustomerRepository;
        private readonly IRepository<Car> _carRepository;
        private readonly IRepository<UserRole> _UserRoleRepository;
        public CustomerController(ICustomerRepository customerRepository, IRepository<Car> carRepository, IRepository<UserRole> UserRoleRepository)
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
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginFormVM)
        {
            if (ModelState.IsValid)
            {
                var customer = await _CustomerRepository.GetCustomerByEmailAsync(loginFormVM.Email);

                if (customer != null && customer.Password == loginFormVM.Password)
                {
                    var claims = new List<Claim>{
                        new Claim(ClaimTypes.Name, customer.Email), 
                        new Claim(ClaimTypes.Role, customer.Role.Role), 
                        new Claim("UserId", customer.CustomerId.ToString()) 
                    };

                    // Skapar Identiteten med "claims", andra parametern är att vi ska använda denna identitet med cookie authentication system.
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // skapar authentication properties, detta göra så att cookien ligger kvar om browsern stängs om usern har klickat i "remember me"
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = loginFormVM.RememberMe
                    };

                    // Här loggas Usern in med dem claims och authentication properties som vi skapat
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                            new ClaimsPrincipal(claimsIdentity), authProperties);

                    // vid lyckad authenticering anropas home controllern och index metoden
                    return RedirectToAction("Index", "home");
                }
                else
                {
                    // lägger till ett felmeddelande till modelen, första parameter är vilken property den ska binda till, lämnas den som tom sträng
                    // blir det ett generellt meddelande som inte sparas i nån speciel property, den följer med iallafall.
                    ModelState.AddModelError("", "The Email or Password was incorrect.");
                }
            }
            // returnerar modelen med felmeddelandet om authenticeringen misslyckas
            return View(loginFormVM);
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVm registerForm)
        {
            if (ModelState.IsValid)
            {
                var customer = await _CustomerRepository.GetCustomerByEmailAsync(registerForm.Email);
                if (customer != null)
                {
                    
                    ModelState.AddModelError("", "There is allready an account registred with that email.");
                    return View(registerForm);
                }

                var userRole = await _UserRoleRepository.GetByIDAsync(2);

                Customer newCustomer = new Customer
                {
                    FirstName = registerForm.FirstName,
                    LastName = registerForm.LastName,
                    Email = registerForm.Email,
                    Password = registerForm.Password,
                    Role = userRole
                };

                await _CustomerRepository.AddAsync(newCustomer);

                LoginVM loginForm = new LoginVM
                {
                    Email = registerForm.Email,
                    Password = registerForm.Password,
                    RememberMe = false
                };

                return RedirectToAction("RegisterComplete", "Customer", loginForm);
            }
            else
            {
                return View(registerForm);
            }
            
            
        }
        public IActionResult RegisterComplete(LoginVM registerDetails) {
        
            return View(registerDetails);
            
        }
        public async Task<IActionResult> LoginOrRegisterBeforeBooking(int carId)
        {
           
            var car = await _carRepository.GetByIDAsync(carId);

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
                // skapar ett anonymt objekt som parameter här, methoden som anropas behöver ett namn på värdet som skickas som parameter, carId är både namnet här och i methodens parameter
                return RedirectToAction("ConfirmBooking", "Booking", new { carId = car.CarId });
            }

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginBeforeBook(LoginAndBookVM loginForm)
        {
            // validateAntiForgeryToken används för säkerhet, en token genereas i vyn som sedan skickas hit till post metoden för
            // att säkerställa att det är rätt formulär och användare som skickar datat. Förhindrar attacker där andra källor kan skapa postmetoder utifrån. 

            if (ModelState.IsValid)
            {
              
                var customer = await _CustomerRepository.GetCustomerByEmailAsync(loginForm.Email);

                if (customer != null && customer.Password == loginForm.Password)
                {
                    var claims = new List<Claim>{
                    new Claim(ClaimTypes.Name, customer.Email), 
                    new Claim(ClaimTypes.Role, customer.Role.Role), 
                    new Claim("UserId", customer.CustomerId.ToString()) 
                    };

                    // Skapar Identiteten med "claims", andra parametern är att vi ska använda denna identitet med cookie authentication system.
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // skapar authentication properties, detta göra så att cookien ligger kvar om browsern stängs om usern har klickat i "remember me"
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = loginForm.RememberMe
                    };

                    // Här loggas Usern in med dem claims och authentication properties som vi skapat
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                            new ClaimsPrincipal(claimsIdentity), authProperties);

                    // vid lyckad authenticering anropas home controllern och index metoden
                    return RedirectToAction("ConfirmBooking", "Booking", new { carId = loginForm.CarId });
                }
                else
                {
                    //då man inte kan skicka modelen tillbaka med en Redirect to action så valde jag att använda mig av tempfiler för felmeddelanden

                    TempData["LoginError1"] = "The Email or Password was incorrect.";
                    return RedirectToAction("LoginOrRegisterBeforeBooking", "Customer", new { carId = loginForm.CarId });
                }
            }
            TempData["LoginError2"] = "You have to fill in all the fields and the correct format.";
            return RedirectToAction("LoginOrRegisterBeforeBooking", "Customer", new { carId = loginForm.CarId });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterBeforeBook(RegisterBeforeBookingVM RegisterForm)
        {
            if (ModelState.IsValid)
            {
                var customer = await _CustomerRepository.GetCustomerByEmailAsync(RegisterForm.Email);
                if (customer != null)
                {
                    TempData["ErrorRegister1"] = "There is already an account with that email address.";
                    return RedirectToAction("LoginOrRegisterBeforeBooking", "Customer", new { carId = RegisterForm.CarId });
                }

                var userRole = await _UserRoleRepository.GetByIDAsync(2);

                Customer newCustomer = new Customer
                {
                    FirstName = RegisterForm.FirstName,
                    LastName = RegisterForm.LastName,
                    Email = RegisterForm.Email,
                    Password = RegisterForm.Password,
                    Role = userRole
                };

                await _CustomerRepository.AddAsync(newCustomer);

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
                TempData["ErrorRegister2"] = "You have to fill in all the fields and the correct format.";
            }
            return RedirectToAction("LoginOrRegisterBeforeBooking", "Customer", new { carId = RegisterForm.CarId });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ListAllCustomers()
        {
            var customers = await _CustomerRepository.GetAllAsync();
            return View(customers);
        }

        // GET: CustomerController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CustomerController/Create
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create()
        {
            var customer = new Customer();
            var userRole = await _UserRoleRepository.GetByIDAsync(2);
            var customerBookings = new List<Booking>();
            
            customer.Role = userRole;
            customer.Bookings = customerBookings;

            return View(customer);
        }

        // POST: CustomerController/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Customer newCustomer)
        {
            if (!ModelState.IsValid)
            {
                return View(newCustomer);
            }
            var customer = await _CustomerRepository.GetCustomerByEmailAsync(newCustomer.Email);
            if(customer != null)
            {
                ModelState.AddModelError("", "There is already a customer with that email address.");
                return View(newCustomer);
            }
            // fick problem med at UserRole inte var trackat av EF Core
            _UserRoleRepository.AttachEntity(newCustomer.Role);

            var addedCustomer = await _CustomerRepository.AddAsync(newCustomer);
            
            return RedirectToAction("CustomerCreationConfirmed", new { id = newCustomer.CustomerId });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CustomerCreationConfirmed(int id)
        {
            var addedCustomer = await _CustomerRepository.GetByIDAsync(id);
            return View(addedCustomer);
        }

        // GET: CustomerController/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int id)
        {
            var userRole = await _UserRoleRepository.GetByIDAsync(2);
            var customerToUpdate = await _CustomerRepository.GetByIDAsync(id);
            customerToUpdate.Role = userRole;

            return View(customerToUpdate);
        }

        // POST: CustomerController/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Customer customer)
        {
            
            if (!ModelState.IsValid)
            {
                return View(customer);
            }
            var updatedCustomer = await _CustomerRepository.UpdateAsync(customer);
            return RedirectToAction("CustomerUpdateConfirmed", new { id = updatedCustomer.CustomerId });
           
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CustomerUpdateConfirmed(int id)
        {
            var updatedCustomer = await _CustomerRepository.GetByIDAsync(id);
            return View(updatedCustomer);
        }

        // GET: CustomerController/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var customerToDelete = await _CustomerRepository.GetByIDAsync(id);
            return View(customerToDelete);
        }

        // POST: CustomerController/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteFromDatabase(int id)
        {
            var CustomerToDelete = await _CustomerRepository.GetByIDAsync(id); 

            try
            {
                await _CustomerRepository.DeleteAsync(CustomerToDelete);
                return RedirectToAction(nameof(ListAllCustomers));
            }
            catch
            {
                return View();
            }
        }
    }
}
