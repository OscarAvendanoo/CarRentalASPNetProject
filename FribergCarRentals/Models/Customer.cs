using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.Models
{
    public class Customer
    {

        public int CustomerId { get; set; }
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

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
