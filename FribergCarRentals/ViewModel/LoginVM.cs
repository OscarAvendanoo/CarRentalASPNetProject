using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.ViewModel
{
    public class LoginVM
    {
        // sätter dessa som required då det gör det möjligt för mvc att göra validering av login formuläret
        // sätter även de andra data annotations för att kunna använda validering och formatering i formuläret
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
