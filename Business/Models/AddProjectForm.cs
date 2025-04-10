

using Microsoft.AspNetCore.Http;

namespace Business.Models;

public class AddProjectForm
{
  
    public string Id { get; set; } = null!;
    public string ProjectName { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string? Description { get; set; }

    public IFormFile? ImageFile { get; set; }
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }
    public decimal? Budget { get; set; }

    public string ClientId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public int StatusId { get; set; } 



}
