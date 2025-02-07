using FribergCarRentals.Data;
using FribergCarRentals.ViewModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Reflection;
using FribergCarRentals.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authorization;

namespace FribergCarRentals.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminRepository _AdminRepository;
        private readonly IRepository<UserRole> _UserRoleRepository;
        

        public AdminController(IAdminRepository adminRepository, IRepository<UserRole> userRoleRepository)
        {
            _AdminRepository = adminRepository;
            _UserRoleRepository = userRoleRepository;
            
        }
        // GET: AdminController
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LoginAndBookVM loginForm)
        {
            if (ModelState.IsValid)
            {
             

                var admin = await _AdminRepository.GetAdminByEmailAsync(loginForm.Email);

                if (admin != null && admin.Password == loginForm.Password)
                {
                    var claims = new List<Claim>{
                    new Claim(ClaimTypes.Name, admin.Email), 
                    new Claim(ClaimTypes.Role, admin.Role.Role), 
                    new Claim("UserId", admin.AdminId.ToString()) 
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
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                  
                    // lägger till ett felmeddelande till modelen, första parameter är vilken property den ska binda till, lämnas den som tom sträng
                    // blir det ett generellt meddelande som inte sparas i nån speciel property, den följer med iallafall.
                    ModelState.AddModelError("", "The Email or Password was incorrect.");
                    
                    return View(loginForm);
                }
            }
           

            return View(loginForm);
        }
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ListAllAdmins()
        {

            var Admins = await _AdminRepository.GetAllAsync();
            return View(Admins);
        }

        // GET: AdminController/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminController/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Create(Admin admin)
        {
            var userRole = await _UserRoleRepository.GetByIDAsync(1);
            admin.Role = userRole;

            // Rensar valideringen manuellt flr Admin.Role då denna läggs till efter modelbindingen
            ModelState.Remove(nameof(Admin.Role));

            // Revaliderar modelstatet
            if (!TryValidateModel(admin))
            {
                return View(admin);
            }
   
            var adminFound = await _AdminRepository.GetAdminByEmailAsync(admin.Email);
            if(adminFound == null)
            {
                await _AdminRepository.AddAsync(admin);
                return View("AdminCreationConfirmation", admin);
            }

            ModelState.AddModelError("", "An Admin with that email address allready exists.");
            return View(admin);

        }

        // GET: AdminController/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdminController/Edit/5
        [Authorize(Roles = "Admin")]
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

        // GET: AdminController/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminController/Delete/5
        [Authorize(Roles = "Admin")]
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
