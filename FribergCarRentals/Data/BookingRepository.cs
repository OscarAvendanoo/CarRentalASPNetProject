using FribergCarRentals.Models;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentals.Data
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _context = applicationDbContext;
        }
        public async Task<IEnumerable<Booking>> GetBookingByUserIDAsync(int Id)
        {
            return await _context.Bookings.Where(b => b.CustomerId == Id).Include(b => b.Car).Include(b => b.Customer).ToListAsync();
           
        }
        public async Task<Booking> GetBookingByIdIncludeCustomerAndCarAsync(int id)
        {
            return await _context.Bookings.Where(b => b.BookingId == id).Include(b => b.Car).Include(b => b.Customer).FirstOrDefaultAsync();
           
        }
    }
}
