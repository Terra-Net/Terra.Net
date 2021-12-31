using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terra.Net.Crypto.Implemetations;

namespace Terra.Net.Objects.Addresses
{
  
    /**
    * `terravaloper-` prefixed validator operator address
    */

  
    public class ValidatorAddress
    {
        private ValidatorAddress(string value)
        {
            Value = value;
        }


        public string Value { get; set; }

        public static ValidatorAddress New(string value)
        {
            return new ValidatorAddress(value);
        }


        /**
         * Checks if a string is a Terra validator address.
         *
         * @param data string to check
         */
        public bool Validate()
        {
            return Validate(Value);
        }

        public bool Validate(string value)
        {
            return Bech32Helper.CheckPrefixAndLength("terravaloper", value, 51);
        }


        /**
        * Converts a Terra account address to a validator address.
        * @param address account address to convert
        */
        public static ValidatorAddress FromAccAddress(AccountAddress address)
        {
            var vals = Bech32.Decode(address.Value);
            return new ValidatorAddress(Bech32.Encode("terravaloper", vals.words));
        }
    }
}
