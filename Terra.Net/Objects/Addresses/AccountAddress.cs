using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terra.Net.Crypto.Implemetations;

namespace Terra.Net.Objects.Addresses
{  
    public class AccountAddress
    {
        public AccountAddress(string value)
        {
            Value = value;
        }
        public string Value { get; }

        /// <summary>
        /// Checks if a string is a valid Terra account address.
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            return Validate(Value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Validate(string value)
        {
            return Bech32Helper.CheckPrefixAndLength("terra", value, 44);
        }

        /// <summary>
        /// Converts a validator address into an account address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static AccountAddress FromValAddress(ValidatorAddress address)
        {
            var vals = Bech32.Decode(address.Value);
            return new AccountAddress(Bech32.Encode("terra", vals.words));
        }
    }
}
