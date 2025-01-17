using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.ViewModel
{
    public class LoginAndBookVM
    {
        // sätter dessa som required då det gör det möjligt för mvc att göra validering av login formuläret
        // sätter även de andra data annotations för att kunna använda validering och formatering i formuläret
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
        public int CarId { get; set; }
    }
}
