using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terra.Net.Common;

namespace Terra.Net.Objects.Dto
{
    public class MsgExecuteContractDto<TExecuteMessage>
    {
        public AccountAddress Sender { get; set; }
        public AccountAddress Contract { get;set; }
        public TExecuteMessage ExecuteMessage { get; set; }

    }
  
}
