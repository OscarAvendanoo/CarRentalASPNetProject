using System.Globalization;

namespace FribergCarRentals.Models
{
    public class Car
    {
        public int CarId { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string ModelYear { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int PricePerDay { get; set; }
        public bool IsAvailable { get; set; }
        public string[] carImages { get; set; } = new string[]{ };

        public virtual ICollection<Booking> Bookings { get; set; }
        
    }
}
