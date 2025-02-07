using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.ViewModel
{
    public class BookingEditViewModel
    {
        public int BookingId { get; set; }
        public int CarId { get; set; }  
        public int CustomerId { get; set; } 
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        
    }
}
