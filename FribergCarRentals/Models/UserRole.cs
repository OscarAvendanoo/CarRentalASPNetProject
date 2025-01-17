using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.Models
{
    public class UserRole
    {
        [Required]
        public int UserRoleId { get; set; }
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
