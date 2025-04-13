using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace Data.Entities;

[Index(nameof(Email), IsUnique = true)]
public class ClientEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ClientName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public virtual ICollection<ProjectEntity> Projects { get; set; } = [];
    public string? Location { get; set; }
    public string? Phone { get; set; }
    public string? ImageUrl { get; set; }
}
  