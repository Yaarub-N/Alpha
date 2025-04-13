namespace Domain.Responses;

public class UserResult : BaseResult
{

}

public class UserResult<User> : UserResult
{
    public User? Result { get; set; }
}
