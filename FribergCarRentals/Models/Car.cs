using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace FribergCarRentals.Models
{
    public class Car
    {
        public int CarId { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Brand can not be empty.")]
        public string Brand { get; set; } = string.Empty;
        [Required]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Model can not be empty.")]
        public string Model { get; set; } = string.Empty;
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Model year can not be empty.")]
        [Required]
        public string ModelYear { get; set; } = string.Empty;
        [Required]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Color can not be empty.")]
        public string Color { get; set; } = string.Empty;
        [Required]
        public int PricePerDay { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public bool IsAvailable { get; set; }
        public string[]? carImages { get; set; } = new string[]{ };

        public virtual ICollection<Booking>? Bookings { get; set; }
        
    }
}
