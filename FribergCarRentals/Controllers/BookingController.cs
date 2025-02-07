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
        private readonly IBookingRepository _bookingRepository;
        private readonly ICustomerRepository _customerRepository;
        public BookingController(IRepository<Car> carRepository,IBookingRepository bookingRepository,ICustomerRepository customerRepository)
        {
            this._carRepository = carRepository;
            this._bookingRepository = bookingRepository;
            this._customerRepository = customerRepository;
        }

        public async Task<IActionResult> UnbookBooking(int bookingId)
        {
            var bookingToUnbook = await _bookingRepository.GetBookingByIdIncludeCustomerAndCarAsync(bookingId);
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
        public async Task<IActionResult> DeleteConfirmed(int bookingId)
        {
            var bookingToDelete = await  _bookingRepository.GetByIDAsync(bookingId);

            // Om bokningen är null eller är en passerad bokning returneras not found
            if (bookingToDelete == null || bookingToDelete.StartDate < DateTime.Today)
            {
                return NotFound();
            }
            await _bookingRepository.DeleteAsync(bookingToDelete);

            // redirect to action ropar på en method i samma controller
            return RedirectToAction(nameof(ListUserBookings));

            
        }
        [Authorize(Roles = "Customer")]
        [HttpGet]
        [Route("ListUserBookings")]
        public async Task<IActionResult> ListUserBookings()
        {
            var userBookings = await _bookingRepository.GetBookingByUserIDAsync(int.Parse(User.FindFirstValue("UserId")));
            return View(userBookings);   
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("ListUserBookings/{id:int}")]
        public async Task<IActionResult> ListUserBookings(int id)
        {
            var userBookings = await _bookingRepository.GetBookingByUserIDAsync(id);
            return View(userBookings);
        }

        // GET: BookingController
        public async Task<ActionResult> ListAllCars()
        {
            var cars = await _carRepository.GetAllAsync();
            return View(cars);
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> ConfirmBooking(int carId)
        {
            var car = await _carRepository.GetByIDAsync(carId);

            if (car == null)
            {
                return NotFound("Car not found.");
            }
            var bookingConfirmationVM = new BookingConfirmationVM
            {
                CarId = car.CarId,
                Brand = car.Brand,
                Model = car.Model,
                ModelYear = car.ModelYear,
                PricePerDay = car.PricePerDay,
                ImageUrl = car.carImages[0] 
            };
            return View(bookingConfirmationVM);
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteBooking(BookingConfirmationVM bookingDetails)
        {
            if (!ModelState.IsValid)
            {
               
                return View("ConfirmBooking", bookingDetails);
            }

           
            if (bookingDetails.StartDate >= bookingDetails.EndDate)
            {
                ModelState.AddModelError("", "The start date must be before the end date.");
                return View("ConfirmBooking", bookingDetails);
            }
            
            var car = await _carRepository.GetByIDAsync(bookingDetails.CarId);
            var customer = await _customerRepository.GetByIDAsync(int.Parse(User.FindFirstValue("UserId")));

            var booking = new Booking
            {
                CarId = car.CarId,
                CustomerId = int.Parse(User.FindFirstValue("UserId")),
                StartDate = bookingDetails.StartDate,
                EndDate = bookingDetails.EndDate,
                TotalCost = (bookingDetails.EndDate - bookingDetails.StartDate).Days * car.PricePerDay
            };

            await _bookingRepository.AddAsync(booking);
            
            return RedirectToAction("BookingSummary", new { bookingId = booking.BookingId });
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> BookingSummary(int bookingId)
        {
            
            var booking = await _bookingRepository.GetBookingByIdIncludeCustomerAndCarAsync(bookingId);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

           
            var summaryVM = new BookingSummaryVm
            {
                BookingId = booking.BookingId,
                Brand = booking.Car.Brand,
                Model = booking.Car.Model,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                TotalPrice = booking.TotalCost,
                CustomerName = $"{booking.Customer.FirstName} {booking.Customer.LastName}",
                CustomerEmail = booking.Customer.Email,
                ImageUrl = booking.Car.carImages[0]
            };

            return View(summaryVM);
        }


        // GET: BookingController/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int id)
        {
            var booking = await _bookingRepository.GetByIDAsync(id);
            var bookingToEditVM = new BookingEditViewModel { BookingId = id, 
                                                             CustomerId = booking.CustomerId,
                                                             CarId = booking.CarId};
            return View(bookingToEditVM);
        }

        // POST: BookingController/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(BookingEditViewModel bookingToUpdateVM)
        {

            if (!ModelState.IsValid)
            {
                return View(bookingToUpdateVM);
            }
            var car = await _carRepository.GetByIDAsync(bookingToUpdateVM.CarId);
            if (car == null)
            {
                ModelState.AddModelError("", "A car with that ID could not be found");
                return View();
            }
            var customer = await _customerRepository.GetByIDAsync(bookingToUpdateVM.CustomerId);
            if (customer == null)
            {
                ModelState.AddModelError("", "A customer with that ID could not be found");
                return View(bookingToUpdateVM);
            }
            if (bookingToUpdateVM.StartDate >= bookingToUpdateVM.EndDate)
            {
                ModelState.AddModelError("", "The start date must be before the end date.");
                return View(bookingToUpdateVM);
            }
            var bookingToUpdate = await _bookingRepository.GetByIDAsync(bookingToUpdateVM.BookingId);
            bookingToUpdate.StartDate = bookingToUpdateVM.StartDate;
            bookingToUpdate.EndDate = bookingToUpdateVM.EndDate;
            bookingToUpdate.CarId = bookingToUpdateVM.CarId;
            bookingToUpdate.CustomerId = bookingToUpdateVM.CustomerId;
            bookingToUpdate.Car = car;
            bookingToUpdate.Customer = customer;
            bookingToUpdate.TotalCost = (bookingToUpdateVM.EndDate - bookingToUpdateVM.StartDate).Days * car.PricePerDay;

            var updatedBooking = await _bookingRepository.UpdateAsync(bookingToUpdate);

            return RedirectToAction("BookingUpdateConfirmed", new { id = bookingToUpdate.BookingId });
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BookingUpdateConfirmed(int id)
        {
            var updatedBooking= await _bookingRepository.GetBookingByIdIncludeCustomerAndCarAsync(id);
            return View(updatedBooking);
        }


        // GET: BookingController/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var bookingToDelete = await _bookingRepository.GetBookingByIdIncludeCustomerAndCarAsync(id);

            if (bookingToDelete == null)
            {
              
                return NotFound();
            }
            else
            {
                return View(bookingToDelete);
            }

        }


        // POST: BookingController/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteFromDatabase(int id)
        {
            var BookingToDelete = await _bookingRepository.GetByIDAsync(id);

            try
            {
                await _bookingRepository.DeleteAsync(BookingToDelete);
                return RedirectToAction("ListAllCustomers", "Customer");
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
