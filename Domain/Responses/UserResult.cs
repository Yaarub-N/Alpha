namespace Domain.Responses;

public class UserResult : BaseResult
{

}

public class UserResult<T> : UserResult
{
    public T? Result { get; set; }
}
