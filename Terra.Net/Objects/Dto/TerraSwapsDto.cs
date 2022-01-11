using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terra.Net.Converters;

namespace Terra.Net.Objects.Dto
{
    public class TerraSwapMessage
    {
        public static TerraSwapMessage FromNativeTokenToBuy(decimal beliefPrice, string denom, ulong amount, decimal maxSpread = 0.01m)
        {
            return new TerraSwapMessage()
            {
                Swap = new Swap() { BeliefPrice = beliefPrice.ToString(CultureInfo.InvariantCulture), MaxSpread = maxSpread.ToString(CultureInfo.InvariantCulture), OfferAsset = new OfferAsset() { Amount = amount, Info = new Info() { NativeToken = new NativeToken() { Denom = denom } } } }
            };
        }
        public static TerraSwapMessage FromTokenToSell(decimal beliefPrice, decimal maxSpread = 0.01m)
        {
            //{"swap":{"belief_price":"0.038454113859554809","max_spread":"0.01"}}
            return new TerraSwapMessage()
            {
                Swap = new Swap() 
                {
                    BeliefPrice = beliefPrice.ToString(CultureInfo.InvariantCulture),
                    MaxSpread = maxSpread.ToString(CultureInfo.InvariantCulture) 
                }
            };
        }
        [JsonProperty("swap")]
        public Swap Swap { get; set; }
    }

    public class Swap
    {
        [JsonProperty("max_spread")]
        public string MaxSpread { get; set; }

        [JsonProperty("offer_asset", NullValueHandling = NullValueHandling.Ignore)]
        public OfferAsset OfferAsset { get; set; }

        [JsonProperty("belief_price")]
        public string BeliefPrice { get; set; }

    }
    public class Asset
    {
        [JsonProperty("info")]
        public Info Info { get; set; }

        [JsonProperty("amount")]
        [JsonConverter(typeof(ParseUlongToStringConverter))]
        public ulong Amount { get; set; }
    }

    public class Info
    {
        [JsonProperty("native_token", NullValueHandling = NullValueHandling.Ignore)]
        public NativeToken NativeToken { get; set; }

        [JsonProperty("token", NullValueHandling = NullValueHandling.Ignore)]
        public Token Token { get; set; }
    }

    public class NativeToken
    {
        [JsonProperty("denom")]
        public string Denom { get; set; }
    }

    public class Token
    {
        [JsonProperty("contract_addr")]
        public string ContractAddress { get; set; }
    }
    public class OfferAsset
    {
        [JsonProperty("amount")]
        [JsonConverter(typeof(ParseUlongToStringConverter))]
        public ulong Amount { get; set; }

        [JsonProperty("info")]
        public Info Info { get; set; }
    }

    public class TerraSellSwapMessage
    {
        public TerraSellSwapMessage(string contract, ulong amount, decimal beliefPrice, decimal maxSpread = 0.01m)
        {
            Send = new SendToSwap<Swap>(TerraSwapMessage.FromTokenToSell(beliefPrice, maxSpread).Swap, amount, contract);
        }
        [JsonProperty("send")]
        public SendToSwap<Swap> Send { get; set; }
    }

    public class SendToSwap<T>
    {
        /*
{
  "send": {
    "msg": "eyJzd2FwIjp7ImJlbGllZl9wcmljZSI6IjAuMDM4NDU0MTEzODU5NTU0ODA5IiwibWF4X3NwcmVhZCI6IjAuMDEifX0=",
    "amount": "100000",
    "contract": "terra17rvtq0mjagh37kcmm4lmpz95ukxwhcrrltgnvc"
  }
}*/
        public SendToSwap(T msg, ulong amount, string contract)
        {
            Msg = msg;
            Amount = amount;
            Contract = contract ?? throw new ArgumentNullException(nameof(contract));
        }

        [JsonProperty("msg")]
        [JsonConverter(typeof(Base64JsonConverter))]
        public T Msg { get; set; }

        [JsonProperty("amount")]
        [JsonConverter(typeof(ParseUlongToStringConverter))]
        public ulong Amount { get; set; }

        [JsonProperty("contract")]
        public string Contract { get; set; }
    }
}
