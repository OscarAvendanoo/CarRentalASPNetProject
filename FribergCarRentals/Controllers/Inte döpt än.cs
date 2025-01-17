using FribergCarRentals.Data;
using FribergCarRentals.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FribergCarRentals.Controllers
{
    public class intedöptänController : Controller
    {
        private readonly BookingRepository _bookingRepository;
        public intedöptänController(BookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }
        // GET: BookingController
        public ActionResult Index()
        {
            // Hämtar ID från den inloggade usern
            var userId = User.FindFirstValue("UserId"); 
            int userIntId = int.Parse(userId);

            var bookings = _bookingRepository.GetBookingByUserID(userIntId);
            return View(bookings);
        }

        // GET: BookingController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BookingController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BookingController/Create
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

        // GET: BookingController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BookingController/Edit/5
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

        // GET: BookingController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BookingController/Delete/5
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
