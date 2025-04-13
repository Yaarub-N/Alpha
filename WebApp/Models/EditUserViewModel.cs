using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class EditUserViewModel
    {

        [Required]
        public string Id { get; set; } = null!;


        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Text)]
        public string? FirstName { get; set; }




        [DataType(DataType.Text)]
        public string? LastName { get; set; }



        [Display(Name = "Role", Prompt = "Select role")]
        public string? Role { get; set; }




        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email", Prompt = "Enter email address")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email")]
        public string Email { get; set; } = null!;

        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }


        [DataType(DataType.Upload)]
        [Display(Name = "Project Image", Prompt = "Select project image")]
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }

       
    }
}
