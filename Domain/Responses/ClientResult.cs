using Domain.Models;


namespace Domain.Responses;




public class ClientResult<T> : BaseResult
{
    public T? Result { get; set; }  
}

public class ClientResult : BaseResult
{

}