namespace FribergCarRentals.ViewModel
{
    public class BookingSummaryVm { 

        public int BookingId { get; set; } 
        public string Brand { get; set; }
        public string Model { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
    
    }
}
