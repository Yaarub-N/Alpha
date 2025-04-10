using Microsoft.AspNetCore.Http;

public class UpdateProjectForm
{
    public string Id { get; set; } = null!;

    public IFormFile? ImageFile { get; set; }  // för uppladdad bild
    public string? ImageUrl { get; set; }

    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; }

    public DateTime? StartDate { get; set; } 
    public DateTime? EndDate { get; set; } 
    public decimal? Budget { get; set; }

    public string ClientId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public int StatusId { get; set; } 
}
