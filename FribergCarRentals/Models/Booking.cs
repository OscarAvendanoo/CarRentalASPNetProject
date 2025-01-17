using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FribergCarRentals.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int CarId { get; set; }  // Foreign key
        public int CustomerId { get; set; } // Foreign key
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [ForeignKey("CarId")]
        public Car Car { get; set; }  
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        public decimal TotalCost { get; set; } = 0;

    }
}
