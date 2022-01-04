#nullable enable
using System;
using Terra.Net.Crypto;
using Terra.Net.Crypto.Ecdsa;
using Terra.Net.Crypto.Implemetations;

using Terra.Net.Crypto.Ripemd160;


namespace Terra.Net.Crypto
{
    /**
     * Abstract key interface that provides transaction signing features and Bech32 address
     * and public key derivation from a public key. This allows you to create custom key
     * solutions, such as for various hardware wallets, by implementing signing and calling
     * `super` with the raw public key from within your subclass. See [[MnemonicKey]] for
     * an implementation of a basic mnemonic-based key.
     */

    public abstract class Key
    {
        private const string BECH32_PUBKEY_DATA_PREFIX = "eb5ae98721";

        public byte[]? PublicKey { get; set; }
        public byte[]? PrivateKey { get; set; }

        public byte[]? RawAddress { get; set; }
        public byte[]? RawPubKey { get; set; }

        /// <summary>
        /// Gets a raw address from a compressed bytes public key.
        /// </summary>
        /// <param name="publicKey">raw public key</param>
        /// <returns></returns>
        public static byte[] AddressFromPublicKey(byte[] publicKey)
        {
            var hash = Ripemd160Manager.GetHash(Sha256Manager.GetHash(publicKey));
            return Bech32.ToWords(hash);
        }


        /// <summary>
        /// Gets a bech32-words pubkey from a compressed bytes public key.
        /// </summary>
        /// <param name="publicKey">raw public key</param>
        /// <returns></returns>
        public static byte[] PubKeyFromPublicKey(byte[] publicKey)
        {
            var buffer = BECH32_PUBKEY_DATA_PREFIX.ToByteArrayFromHex();
            var rv = new byte[buffer.Length + publicKey.Length];
            Buffer.BlockCopy(buffer, 0, rv, 0, buffer.Length);
            Buffer.BlockCopy(publicKey, 0, rv, buffer.Length, publicKey.Length);
            return Bech32.ToWords(rv);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;
            if (!(obj is Key))
            {
                return false; 
            }
            return (obj as Key).RawPubKey.SequenceEqual(RawPubKey);
        }
        /// <summary>
        ///  You will need to supply `sign`, which produces a signature for an arbitrary bytes payload with the ECDSA curve secp256pk1.
        /// </summary>
        /// <param name="payload">payload the data to be signed</param>
        /// <returns></returns>
        public abstract byte[] Sign(byte[] payload);



        protected Key(byte[] publicKey)
        {
            this.RawAddress = AddressFromPublicKey(publicKey);
            this.RawPubKey = PubKeyFromPublicKey(publicKey);
        }

        protected Key()
        {
        }

        //private byte[] CreateSignature(SignDoc sign)
        //{
        //    var docSerialized = sign.ToByteArray();
        //    var signature = Sign(docSerialized);
        //    return signature;
        //}
        //public TxRaw ToRawSignedTx(SignDoc doc)
        //{
        //    var signature = CreateSignature(doc);
        //    var tx = new TxRaw()
        //    {
        //        AuthInfoBytes = doc.AuthInfoBytes,
        //        BodyBytes = doc.BodyBytes,
        //    };
        //    tx.Signatures.Add(ByteString.CopyFrom(signature));
        //    return tx;
        //}
    }
}