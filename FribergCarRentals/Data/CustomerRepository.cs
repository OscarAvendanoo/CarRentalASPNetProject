using FribergCarRentals.Models;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentals.Data
{
    public class CustomerRepository : GenericRepository<Customer>
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _context = applicationDbContext;
        }
        public Customer GetCustomerByEmail(string email)
        {
            var customer = _context.Customers.Include(u => u.Role).FirstOrDefault(u => u.Email == email);
            return customer;
        }
    }
}
