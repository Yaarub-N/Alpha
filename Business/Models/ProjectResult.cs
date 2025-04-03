using Domain.Models;

namespace Business.Models
{
    public class ProjectResult<T> : BaseResult
    {
        public IEnumerable<T>? Result { get; set; }
    }
    
    public class ProjectResult : BaseResult
    {
        
    }
}
