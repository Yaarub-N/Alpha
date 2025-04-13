using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class UserViewModel
{
    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = null!;

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [Phone]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = null!;


    [Display(Name = "Job Title ")]
    public string Role { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [DataType(DataType.Upload)]
    [Display(Name = "Image")]
    public string? ImgUrl { get; set; }


}
