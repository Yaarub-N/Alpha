using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models;

public class ClientResult: BaseResult
{
    public IEnumerable<Client>? Result { get; set; }

}
