namespace Domain.Responses
{
    public class ProjectResult<T> : BaseResult
    {
        public T? Result { get; set; }  // Changed from IEnumerable<T> to T
    }

    public class ProjectResult : BaseResult
    {

    }
}