using FribergCarRentals.Models;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentals.Data
{
    public class CarRepository : GenericRepository<Car>
    {
        private readonly ApplicationDbContext _context;

        public CarRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _context = applicationDbContext;
        }

    }
}
