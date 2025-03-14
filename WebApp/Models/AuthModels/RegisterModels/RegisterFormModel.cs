using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.AuthModels.RegisterModels
{
    public class RegisterFormModel
    {
        [Display(Name = "First Name",Prompt ="Enter your first name")]
        [Required(ErrorMessage ="You must enter your first name")]
        public string FirstName { get; set; } = null!;

        [Display(Name = "Last Name", Prompt = "Enter your last first name")]
        [Required(ErrorMessage = "You must enter your last name")]
        public string LastName { get; set; } = null!;

        [Display(Name = "Email", Prompt = "Enter your email")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",ErrorMessage ="You must enter a valid email address")]
        [Required(ErrorMessage = "You must enter your Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;

        
        [Display(Name = "Password", Prompt = "Enter your password")]
        [RegularExpression(@"^(?=.*[a-ö])(?=.*[A-Ö])(?=.*\d)(?=.*[\W_]).{8,}$",ErrorMessage ="You must enter a strong password")]
        [Required(ErrorMessage = "You must enter your password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name ="Confirm Password", Prompt = "Enter your password")]
        [Required(ErrorMessage = "You must confirm your password")]
        [DataType(DataType.EmailAddress)]
        [Compare(nameof(Password),ErrorMessage ="Your password do not mutch!")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
