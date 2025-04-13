using System.ComponentModel.DataAnnotations;


namespace WebApp.Models
{
    public class AddClientForm
    {
        [DataType(DataType.Text)]
        [Display(Name = "Client Name", Prompt = "Enter client name")]
        [Required(ErrorMessage = "Required")]
        public string ClientName { get; set; } = null!;

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email", Prompt = "Enter email address")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email")]
        public string Email { get; set; } = null!;


 
        [Display(Name = "Phone", Prompt = "Enter phone number (optional)")]
        public string? Phone { get; set; }


 
        [Display(Name = "Location", Prompt = "Enter location (optional)")]
        public string? Location { get; set; }


        [DataType(DataType.Upload)]
        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }

    }
}
