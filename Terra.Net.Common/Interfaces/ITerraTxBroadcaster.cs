using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terra.Net.Common.Interfaces
{
    public interface ITerraTxBroadcaster
    {
        Task BroadcastTx(byte[] transactionData);
    }
}
