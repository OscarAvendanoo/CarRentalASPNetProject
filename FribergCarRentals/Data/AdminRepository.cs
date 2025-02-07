using FribergCarRentals.Models;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentals.Data
{
    public class AdminRepository : GenericRepository<Admin> , IAdminRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _context = applicationDbContext;
        }
        public async Task<Admin> GetAdminByEmailAsync(string email)
        {
            return await _context.Admins.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email); 
        }

    }
}

