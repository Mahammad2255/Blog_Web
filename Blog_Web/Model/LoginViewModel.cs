using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Blog_Web.Model
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]

        public string? Email { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "{0} at least {2} charachter", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

    }
}
