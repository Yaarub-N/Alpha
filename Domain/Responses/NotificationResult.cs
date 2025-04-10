namespace Domain.Responses;

public class NotificationResult : BaseResult
{

}

public class NotificationResult<T> : NotificationResult
{
    public T? Result { get; set; }
}