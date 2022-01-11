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
        public string Sender { get; set; }
        public string Contract { get;set; }
        public TExecuteMessage ExecuteMessage { get; set; }

    }
  
}
