using FribergCarRentals.Models;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentals.Data
{
    public class BookingRepository : GenericRepository<Booking>
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _context = applicationDbContext;
        }
        public IEnumerable<Booking> GetBookingByUserID(int Id)
        {
            var bookings = _context.Bookings.Where(b => b.CustomerId == Id).Include(b => b.Car).Include(b => b.Customer).ToList();
            return bookings;
        }
        public Booking GetBookingByIdIncludeCustomerAndCar(int id)
        {
            var booking = _context.Bookings.Where(b => b.BookingId == id).Include(b => b.Car).Include(b => b.Customer).FirstOrDefault();
            return booking;
        }
    }
}
