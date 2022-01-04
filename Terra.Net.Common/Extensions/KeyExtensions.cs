using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terra.Net.Crypto.Implemetations;

namespace Terra.Net.Common.Extensions
{
    public static class KeyExtensions
    {
        /// <summary>
        /// Terra account address. `terra-` prefixed.
        /// </summary>
        public static AccountAddress GetAccountAddress(byte[] rawAddres)
        {
            return new AccountAddress(Bech32.Encode("terra", rawAddres));
        }

        /// <summary>
        /// Terra validator address. `terravaloper-` prefixed.
        /// </summary>
        public static ValidatorAddress GetValidatorAddress(byte[] rawAddres)
        {
            return ValidatorAddress.New(Bech32.Encode("terravaloper", rawAddres));
        }
    }
}
