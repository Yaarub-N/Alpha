

namespace Data.Models
{
    public class RepositoryResult<TResult>
    {
        public TResult? Result { get; set; }
        public bool Succeeded { get; set; }
        public string? ErrorMessage { get; set; }
        public int statusCode { get; set; } 

    }


}
