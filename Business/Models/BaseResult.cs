namespace Business.Models;

public abstract class BaseResult
{
    public bool Succeeded { get; set; }
    public string? ErrorMessage { get; set; }
    public int StatusCode { get; set; }
}
