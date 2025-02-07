using FribergCarRentals.Models;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentals.Data
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task<IEnumerable<Booking>> GetBookingByUserIDAsync(int Id);

        Task<Booking> GetBookingByIdIncludeCustomerAndCarAsync(int id);
    }
}
