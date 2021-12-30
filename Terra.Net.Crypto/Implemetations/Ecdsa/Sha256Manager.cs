using System;
using Terra.Net.Crypto.Ecdsa.Internal.Sha256;

namespace Terra.Net.Crypto.Ecdsa
{
    public class Sha256Manager
    {
        private readonly Sha256T _sha;

        public Sha256Manager()
        {
            _sha = new Sha256T();
            Internal.Sha256.Hash.Initialize(_sha);
        }


        public void Write(byte[] data)
        {
            Internal.Sha256.Hash.Write(_sha, data, (UInt32)data.Length);
        }

        public void Write(byte[] data, int len)
        {
            Internal.Sha256.Hash.Write(_sha, data, (UInt32)len);
        }

        public byte[] FinalizeAndGetResult()
        {
            byte[] outputSer = new byte[32];
            Internal.Sha256.Hash.Finalize(_sha, outputSer);
            return outputSer;
        }


        
        public static byte[] GetHash(byte[] data)
        {
            Sha256T sha = new Sha256T();
            Internal.Sha256.Hash.Initialize(sha);
            Internal.Sha256.Hash.Write(sha, data, (UInt32)data.Length);
            byte[] outputSer = new byte[32];
            Internal.Sha256.Hash.Finalize(sha, outputSer);
            return outputSer;
        }
    }
}
