using FribergCarRentals.Models;

namespace FribergCarRentals.Data
{
    public interface IAdminRepository : IRepository<Admin>
    {
        Task<Admin> GetAdminByEmailAsync(string email);
    }
}
