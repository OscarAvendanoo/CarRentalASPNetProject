using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.Models
{
    public class Customer
    {

        public int CustomerId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "First name cannot be empty.")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Last name cannot be empty.")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The password must be between 6-20 characters long.")]
        public string Password { get; set; }
        [Required]
        public UserRole Role { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
