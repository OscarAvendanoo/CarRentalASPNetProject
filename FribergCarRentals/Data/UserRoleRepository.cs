using FribergCarRentals.Data;
using FribergCarRentals.Models;

namespace FribergCarRentals.Data
{
    public class UserRoleRepository: GenericRepository<UserRole>
    {
        private readonly ApplicationDbContext _context;
        public UserRoleRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _context = applicationDbContext;
        }
    }
}
