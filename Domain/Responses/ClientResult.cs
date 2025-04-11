using Domain.Models;


namespace Domain.Responses;




public class ClientResult<Client> : BaseResult
{
    public Client? Result { get; set; }  
}

public class ClientResult : BaseResult
{

}