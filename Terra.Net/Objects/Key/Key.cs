#nullable enable
using System;
using Terra.Net.Crypto;
using Terra.Net.Crypto.Ecdsa;
using Terra.Net.Crypto.Implemetations;
//using TerraSdk.Common.Helpers;
//using TerraSdk.Core;
//using TerraSdk.Core.Account;
//using Terra.Net.Crypto.Bech32;
//using Terra.Net.Crypto.Ecdsa;
using Terra.Net.Crypto.Ripemd160;
using Terra.Net.Objects.Addresses;

namespace Terra.Net.Objects
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

        /**
         * Gets a raw address from a compressed bytes public key.
         *
         * @param publicKey raw public key
         */
        public static byte[] AddressFromPublicKey(byte[] publicKey)
        {
            var hash = Ripemd160Manager.GetHash(Sha256Manager.GetHash(publicKey));
            return Bech32.ToWords(hash);
        }

        /**
         * Gets a bech32-words pubkey from a compressed bytes public key.
         *
         * @param publicKey raw public key
         */
        public static byte[] PubKeyFromPublicKey(byte[] publicKey)
        {
            var buffer = BECH32_PUBKEY_DATA_PREFIX.ToByteArrayFromHex();
            var rv = new byte[buffer.Length + publicKey.Length];
            Buffer.BlockCopy(buffer, 0, rv, 0, buffer.Length);
            Buffer.BlockCopy(publicKey, 0, rv, buffer.Length, publicKey.Length);
            return Bech32.ToWords(rv);
        }
        
        /**
         * You will need to supply `sign`, which produces a signature for an arbitrary bytes payload
         * with the ECDSA curve secp256pk1.
         *
         * @param payload the data to be signed
         */
        public abstract byte[] Sign(byte[] payload);

        /**
         * Terra account address. `terra-` prefixed.
         */

        public AccountAddress GetAccountAddress {
            get
            {
                if (this.RawAddress == null)
                {
                    throw new Exception("Could not compute AccAddress: missing rawAddress");
                }
                return new AccountAddress(Bech32.Encode("terra",(byte[])this.RawAddress));
            }
        }

        //    /**
        //     * Terra validator address. `terravaloper-` prefixed.
        //     */
        public ValidatorAddress GetValidatorAddress
        {
            get
            {
                if (this.RawAddress == null)
                {
                    throw new Exception("Could not compute ValAddress: missing rawAddress");
                }
                return ValidatorAddress.New(Bech32.Encode("terravaloper", (byte[])this.RawAddress));
            }
        }

        //    /**
        //     * Terra account public key. `terrapub-` prefixed.
        //     */
        //public AccPubKey AccPubKey
        //{
        //    get
        //    {
        //        if (this.RawPubKey == null)
        //        {
        //            throw new Exception("Could not compute AccPubKey: missing RawPubKey");
        //        }
        //        return AccPubKey.New(Bech32.Encode("terrapub", (byte[])this.RawPubKey));
        //    }
        //}

        //    /**
        //     * Terra validator public key. `terravaloperpub-` prefixed.
        //     */
        //public ValPubKey ValPubKey
        //{
        //    get
        //    {
        //        if (this.RawPubKey == null)
        //        {
        //            throw new Exception("Could not compute ValAddress: missing RawPubKey");
        //        }
        //        return ValPubKey.New(Bech32.Encode("terravaloperpub", (byte[])this.RawPubKey));
        //    }
        //}
        
        //    /**
        //     * Called to derive the relevant account and validator addresses and public keys from
        //     * the raw compressed public key in bytes.
        //     *
        //     * @param publicKey raw compressed bytes public key
        //     */
        //    constructor(public publicKey?: Buffer) {
        //    if (publicKey) {
        //      this.RawAddress = addressFromPublicKey(publicKey);
        //      this.RawPubKey = pubKeyFromPublicKey(publicKey);
        //    }
        //}

        protected Key(byte[] publicKey)
        {
            this.RawAddress = AddressFromPublicKey(publicKey);
            this.RawPubKey = PubKeyFromPublicKey(publicKey);
        }

        protected Key()
        {
        }

        ///**
        // * Signs a [[StdSignMsg]] with the method supplied by the child class.
        // *
        // * @param tx sign-message of the transaction to sign
        // */
        public StdSignature CreateSignature(StdSignMsg tx)
        {
            var json = tx.ToJson();
            var jsonBytes = json.ToByteArrayFromString();
            var sigBuffer = Sign(jsonBytes);

            if (PublicKey == null )
            {
                throw new Exception("Signature could not be created: Key instance missing publicKey");
            }

            var signature=  new StdSignature(Convert.ToBase64String(sigBuffer), new PublicKey("tendermint/PubKeySecp256k1", Convert.ToBase64String(PublicKey)));
            signature.Sequence = tx.Sequence;
            signature.AccountNumber= tx.AccountNumber;
            return signature;
        }

        ///**
        // * Signs a [[StdSignMsg]] and adds the signature to a generated StdTx that is ready to be broadcasted.
        // * @param tx
        // */
        public StdTx2 SignTx2(StdSignMsg tx)
        {
            var sig = CreateSignature(tx);
            return new StdTx2(tx.Msgs, tx.Fee, new  [] {sig}, tx.Memo, tx.TimeoutHeight);
        }

        public StdTx SignTx(StdSignMsg tx)
        {
            var sig = CreateSignature(tx);
            return new StdTx(tx.Msgs, tx.Fee, new[] { sig }, tx.Memo, tx.TimeoutHeight);
        }

    }
}