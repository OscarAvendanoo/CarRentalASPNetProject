using FribergCarRentals.Data;
using FribergCarRentals.Models;
using FribergCarRentals.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace FribergCarRentals.Controllers
{
    public class BookingController : Controller
    {
        private readonly IRepository<Car> _carRepository;
        private readonly BookingRepository _bookingRepository;
        private readonly CustomerRepository _customerRepository;
        public BookingController(IRepository<Car> carRepository,BookingRepository bookingrepository,CustomerRepository customerRepository)
        {
            this._carRepository = carRepository;
            this._bookingRepository = bookingrepository;
            this._customerRepository = customerRepository;
        }

        public IActionResult UnbookBooking(int bookingId)
        {
            var bookingToUnbook = _bookingRepository.GetBookingByIdIncludeCustomerAndCar(bookingId);
            if (bookingToUnbook == null)
            {
                // returnerar en not found sida
                return NotFound();
            }
            else
            {
                return View(bookingToUnbook);
            }
        }
        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult DeleteConfirmed(int bookingId)
        {
            var bookingToDelete = _bookingRepository.GetbyID(bookingId);

            // Om bokningen är null eller är en passerad bokning returneras not found
            if (bookingToDelete == null || bookingToDelete.StartDate < DateTime.Today)
            {
                return NotFound();
            }
            _bookingRepository.Delete(bookingToDelete);

            // redirect to action ropar på en method i samma controller
            return RedirectToAction(nameof(ListUserBookings));

        }
        public IActionResult ListUserBookings()
        {
            var bookings = _bookingRepository.GetBookingByUserID(int.Parse(User.FindFirstValue("UserId")));
            return View(bookings);
        }
        // GET: BookingController
        public ActionResult ListAllCars()
        {
            var cars = _carRepository.GetAll();
            return View(cars);
        }
        //public IActionResult StartBooking(int carId)
        //{
        //    // Hämtar bil och kollar så att ej är null
        //    var car = _carRepository.GetbyID(carId);
        //    if (car == null)
        //    {
        //        return NotFound();
        //    }
        //    // View model för att skicka car id till vyn, skapar även mallen för formuläret.
        //    var model = new StartBookingVM
        //    {
        //        LoginForm = new LoginAndBookVM { CarId = carId },
        //        RegisterForm = new RegisterBeforeBookingVM { CarId = carId }
        //    };
            
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        // skapar ett anonymt objekt som parameter här, methoden som anropas behöver ett namn på värdet som skickas som parameter, carId vara både namnet här och i methodens parameter
        //        return RedirectToAction("ConfirmBooking", "Booking",new { carId = car.CarId });
        //    }
            
        //    return View(model);
        //}

        public IActionResult ConfirmBooking(int carId)
        {
            var car = _carRepository.GetbyID(carId);

            if (car == null)
            {
                // If the car doesn't exist, redirect to an error page or show a message
                return NotFound("Car not found.");
            }
            var bookingConfirmationVM = new BookingConfirmationVM
            {
                CarId = car.CarId,
                Brand = car.Brand,
                Model = car.Model,
                PricePerDay = car.PricePerDay,
                ImageUrl = car.carImages[0] 
            };
            return View(bookingConfirmationVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CompleteBooking(BookingConfirmationVM bookingDetails)
        {
            if (!ModelState.IsValid)
            {
                // If the model is invalid, redisplay the confirmation page with validation messages
                return View("ConfirmBooking", bookingDetails);
            }

            // Validate the rental period (e.g., check for conflicts, ensure dates are valid, etc.)
            if (bookingDetails.StartDate >= bookingDetails.EndDate)
            {
                ModelState.AddModelError("", "The start date must be before the end date.");
                return View("ConfirmBooking", bookingDetails);
            }

            // Proceed with booking logic
            var car = _carRepository.GetbyID(bookingDetails.CarId);
            var customer = _customerRepository.GetbyID(int.Parse(User.FindFirstValue("UserId")));

            var booking = new Booking
            {
                CarId = car.CarId,
                CustomerId = int.Parse(User.FindFirstValue("UserId")), // Retrieve the logged-in user's ID
                StartDate = bookingDetails.StartDate,
                EndDate = bookingDetails.EndDate,
                TotalCost = (bookingDetails.EndDate - bookingDetails.StartDate).Days * car.PricePerDay
            };

            _bookingRepository.Add(booking);
            

            // Redirect to a confirmation or summary page
            return RedirectToAction("BookingSummary", new { bookingId = booking.BookingId });
        }
        public IActionResult BookingSummary(int bookingId)
        {
            // Fetch the booking from the database using the bookingId
            var booking = _bookingRepository.GetBookingByIdIncludeCustomerAndCar(bookingId);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            // Prepare a ViewModel to pass data to the view
            var summaryVM = new BookingSummaryVm
            {
                BookingId = booking.BookingId,
                Brand = booking.Car.Brand,
                Model = booking.Car.Model,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                TotalPrice = booking.TotalCost,
                CustomerName = $"{booking.Customer.FirstName} {booking.Customer.LastName}",
                CustomerEmail = booking.Customer.Email
            };

            return View(summaryVM);
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
