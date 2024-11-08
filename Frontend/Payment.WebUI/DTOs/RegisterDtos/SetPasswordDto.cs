using System.ComponentModel.DataAnnotations;

namespace Payment.WebUI.DTOs.RegisterDtos
{
    public class SetPasswordDto
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

}
