

using Microsoft.AspNetCore.Identity;


namespace Data.Entities;

public class UserEntity : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Jobtitle { get; set; }

    public ICollection<ProjectEntity> Projects { get; set; } = [];
}
