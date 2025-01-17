using FribergCarRentals.Data;
using FribergCarRentals.Models;
using FribergCarRentals.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace FribergCarRentals.Controllers
{
    public class UserController : Controller
    {
        private readonly CustomerRepository _userRepository;

        public UserController(CustomerRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public IActionResult Login()
        {
            return View();
        }
        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        // validateAntiForgeryToken används för säkerhet, en token genereas i vyn som sedan skickas hit till post metoden för
        // att säkerställa att det är rätt formulär och användare som skickar datat. Förhindrar attacker där andra källor kan skapa postmetoder utifrån. 
        public IActionResult Login(LoginVM loginVm)
        {
            if (ModelState.IsValid)
            {
                var customer = _userRepository.GetCustomerByEmail(loginVm.Email);
                
                if(customer != null && customer.Password == loginVm.Password)
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
                        IsPersistent = loginVm.RememberMe 
                    };

                    // Här loggas Usern in med dem claims och authentication properties som vi skapat
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
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
           return View(loginVm);
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        // GET: UserController
        public ActionResult Index()
        {
            return View();
        }

        // GET: UserController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserController/Create
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

        // GET: UserController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserController/Edit/5
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

        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserController/Delete/5
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
