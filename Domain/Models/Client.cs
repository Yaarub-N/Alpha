
namespace Domain.Models
{
    public class Client
    {
        public string Id { get; set; } =null!;
        public string ClientName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; } 
        public string? Location { get; set; }
        public string? ImageUrl { get; set; }

    } 
}
