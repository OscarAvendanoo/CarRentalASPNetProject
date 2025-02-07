using FribergCarRentals.Models;
using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.ViewModel
{
    public class BookingConfirmationVM
    {
        public int CarId { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string ModelYear { get; set; }   
        public decimal PricePerDay { get; set; }
        public string ImageUrl { get; set; }
        [Required(ErrorMessage = "Please select a start date.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Please select an end date.")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
    }
}
