namespace Terra.Net.Objects
{
    public class MnemonicKeyOptions
    {
        /**
         * Space-separated list of words for the mnemonic key.
         */
        public string? Mnemonic { get; set; }

        /**
       * BIP44 account number.
       */
        public int? Account { get; set; } = 0;

        /**
       * BIP44 index number
       */
        public int? Index { get; set; } = 0;

        /**
       * Coin type. Default is LUNA, 330.
       */
        public int? CoinType { get; set; } = 330; // Luna
    }
}