using FribergCarRentals.Data;
using FribergCarRentals.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Linq.Expressions;

namespace FribergCarRentals.Controllers
{
    public class CarController : Controller
    {
        private readonly IRepository<Car> _carRepository;
        public CarController(IRepository<Car> carRepository)
        {
            this._carRepository = carRepository;
        }

        // GET: CarController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var car = await _carRepository.GetByIDAsync(id);
            return View(car);
        }

        // GET: CarController/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            var car = new Car
            {
                carImages = new string[4]
            };

            return View(car);
        }


        // POST: CarController/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Car car)
        {
            if (!ModelState.IsValid)
            {
                return View(car);
            }
            await _carRepository.AddAsync(car);
            return RedirectToAction("ListAllCars", "Booking");

        }

        // GET: CarController/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int id)
        {
            var carToEdit = await _carRepository.GetByIDAsync(id);
    
            return View(carToEdit);
            
        }
        public async Task<ActionResult> CarUpdateConfirmed(int id)
        {
            var updatedCar = await _carRepository.GetByIDAsync(id);
            return View(updatedCar);
        }

        // POST: CarController/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Car car)
        {
            if (!ModelState.IsValid)
            {
                return View(car);
            }
            var updatedCar = await _carRepository.UpdateAsync(car);
            return RedirectToAction("CarUpdateConfirmed",new { id = updatedCar.CarId });
        }

        // GET: CarController/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
           
            var car = await _carRepository.GetByIDAsync(id);
            return View(car);
        }

        // POST: CarCIndex1.cshtmlontroller/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteFromDatabase(int CarId)
        {
           var carToDelete = await _carRepository.GetByIDAsync(CarId);
           await _carRepository.DeleteAsync(carToDelete);
                
           return View("DeleteConfirmed");

        }
    }
}
