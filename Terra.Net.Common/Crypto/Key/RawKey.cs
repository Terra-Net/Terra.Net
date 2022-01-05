using NBitcoin.Secp256k1;
using System;
using Terra.Net.Crypto.Ecdsa;

namespace Terra.Net.Crypto
{
    public class RawKey : Key
    {
        private ECPrivKey? ecPrivateKey;

        public RawKey()
        {
        }

        public RawKey(byte[] privateKey)
        {
            SetPrivate(privateKey);
        }
        
        protected void SetPrivate(byte[] privateKey)
        {
            PrivateKey = privateKey;
            ecPrivateKey = Context.Instance.CreateECPrivKey(privateKey);
            PublicKey = ecPrivateKey.CreatePubKey().ToBytes();
            RawAddress = AddressFromPublicKey(PublicKey);
            RawPubKey = PubKeyFromPublicKey(PublicKey);
        }

        public override byte[] Sign(byte[] payload)
        {
            var hash = Sha256Manager.GetHash(payload);
            ecPrivateKey.TrySignECDSA(hash, out var signature);
            var n = new byte[64];
            if (signature == null)
            {
                throw new Exception("No signature created!");
            }
            signature.WriteCompactToSpan(n);
            return n;
        }

    }
}