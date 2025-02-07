using FribergCarRentals.Models;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentals.Data
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _context = applicationDbContext;
        }
        public async Task<Customer> GetCustomerByEmailAsync(string email)
        {
            return await _context.Customers.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
            
        }
    }
}
