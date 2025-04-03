using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models
{
    public class UserResult : BaseResult
    {
        public IEnumerable<User>? Result { get; set; }
    }

    public class UserResult<T> : BaseResult
    {
        public IEnumerable<T>? Result { get; set; }
    }
  

}
