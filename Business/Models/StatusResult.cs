

using Domain.Models;

namespace Business.Models;

public class StatusResult : BaseResult
{
    public IEnumerable< Status>? Result { get; set; }
}

