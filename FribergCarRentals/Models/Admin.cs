using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.Models
{
    public class Admin
    {
        public int AdminId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public UserRole Role { get; set; }
    }
}
