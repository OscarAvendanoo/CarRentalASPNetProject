using FribergCarRentals.Models;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentals.Data
{
    public class AdminRepository : GenericRepository<Admin>
    {
        private readonly ApplicationDbContext _context;

        public AdminRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _context = applicationDbContext;
        }
        public Admin GetAdminByEmail(string email)
        {
            var admin = _context.Admins.Include(u => u.Role).FirstOrDefault(u => u.Email == email);
            return admin;
        }
    }
}
