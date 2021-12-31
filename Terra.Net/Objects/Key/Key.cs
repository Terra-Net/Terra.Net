#nullable enable
using Cosmos.Crypto.Secp256K1;
using Cosmos.Tx.V1Beta1;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
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
using static Cosmos.Tx.V1Beta1.ModeInfo;

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

        public AccountAddress GetAccountAddress
        {
            get
            {
                if (this.RawAddress == null)
                {
                    throw new Exception("Could not compute AccAddress: missing rawAddress");
                }
                return new AccountAddress(Bech32.Encode("terra", (byte[])this.RawAddress));
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

        /* export function fromProto(pubkeyAny: PublicKey.Proto): PublicKey {
    const typeUrl = pubkeyAny.typeUrl;
    if (typeUrl === '/cosmos.crypto.secp256k1.PubKey') {
      return SimplePublicKey.unpackAny(pubkeyAny);
    } else if (typeUrl === '/cosmos.crypto.multisig.LegacyAminoPubKey') {
      return LegacyAminoMultisigPublicKey.unpackAny(pubkeyAny);
    } else if (typeUrl === '/cosmos.crypto.ed25519.PubKey') {
      return ValConsPublicKey.unpackAny(pubkeyAny);
    }

    throw new Error(`Pubkey type ${typeUrl} not recognized`);
  }*/

        private byte[] CreateSignature(SignDoc sign)
        {
            var docSerialized = sign.ToByteArray();
            var signature = Sign(docSerialized);
            return signature;
        }
        public TxRaw ToRawSignedTx(SignDoc doc)
        {
            var signature = CreateSignature(doc);
            var tx = new TxRaw()
            {
                AuthInfoBytes = doc.AuthInfoBytes,
                BodyBytes = doc.BodyBytes,
            };
            tx.Signatures.Add(ByteString.CopyFrom(signature));
            return tx;
        }

     

    }
}