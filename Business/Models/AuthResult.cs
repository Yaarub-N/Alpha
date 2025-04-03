using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models
{
    public class AuthResult : BaseResult
    {
        public string? SuccessMessage { get; set; }
    }

    public class AuthResult<T> : AuthResult
    {
        public T? Result { get; set; }
    }
}
