using FribergCarRentals.Models;

namespace FribergCarRentals.Data
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer> GetCustomerByEmailAsync(string email);
    }
}
