using NBitcoin;
using Terra.Net.Crypto.Bip39;

namespace Terra.Net.Crypto
{
    public class MnemonicKey : RawKey
    {

        public MnemonicKey(): this(null, 330, 0, 0)
        {
          
        }
        public MnemonicKey(string mnemonic) : this(mnemonic, 330, 0, 0)
        {

        }

        public MnemonicKey(MnemonicKeyOptions options): this(options.Mnemonic, options.CoinType, options.Account, options.Index)
        {

        }

        public MnemonicKey(string? mnemonic = null, int? coinType =330, int? account = 0, int? index = 0)
        {
            var bip39 = new Bip39.Bip39();

            Mnemonic = mnemonic ?? bip39.GenerateMnemonic(256, Bip39Wordlist.English);

            var seed = bip39.MnemonicToSeed(Mnemonic, null);
            var extKey = ExtKey.CreateFromSeed(seed);
            var hdPathLuna = KeyPath.Parse($"m/44'/{coinType}'/{account}'/0/{index}");
            var terraHd = extKey.Derive(hdPathLuna);
            var privateKey = terraHd.PrivateKey.ToBytes();

            SetPrivate(privateKey);
        }

        public string Mnemonic { get; }
    }
}