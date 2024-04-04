using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Blog_Web.Model
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name="Username")]

        public string? UserName { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string? Name { get; set; }
        [Required]
        [Display(Name = "Email")]
        public string? Email { get; set; }
        [Required]
        [StringLength(10, ErrorMessage = "{0} at least {2} charachter", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "{0} at least {2} charachter", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]

        [Display(Name = "Confirm password")]
        public string? ConfirmPassword { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile? ImageFile { get; set; }
    }
}
