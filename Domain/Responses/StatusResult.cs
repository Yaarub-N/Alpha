using Domain.Models;

namespace Domain.Responses;






public class SingleStatusResult : BaseResult
{
    public Status? Result { get; set; }
}


    public class StatusResult<T> : BaseResult
    {
        public T? Result { get; set; }  
    }

   
public class StatusResult : BaseResult
{
    public IEnumerable<Status>? Result { get; set; }
}