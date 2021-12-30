using System;
using System.Security.Cryptography;
using System.Text;
using Terra.Net.Crypto.Chaos.Nacl;

namespace Terra.Net.Crypto
{
    public static class Hash
    {
        static readonly Lazy<Ripemd160.Ripemd160> Ripemd160Lazy = new(() => new Ripemd160.Ripemd160());

        public const int KeyLength = 32;
        public const int NonceLength = 24;
        public const int PublicKeyLength = 32;
        public const int SeedLength = 32;
        public const int SignatureLength = 64;

        //public static SignatureAlgorithm Algorithm { get; } = SignatureAlgorithm.Ed25519;

        public static byte[] EncryptSymmetric(byte[] message, byte[] nonce, byte[] sharedKey)
        {
            return XSalsa20Poly1305.Encrypt(message, sharedKey, nonce);
        }

        public static byte[] DecryptSymmetric(byte[] message, byte[] nonce, byte[] sharedKey)
        {
            return XSalsa20Poly1305.TryDecrypt(message, sharedKey, nonce);
        }

        public static byte[] Sha256(byte[] input)
        {          
            var hash = SHA256.HashData(input);
            return hash;
        }

        public static byte[] Sha256(string input)
        {
            var inputBytes = Encoding.Default.GetBytes(input);

            return Hash.Sha256(inputBytes);
        }

        //public static string Sha256Hex(string inputHex)
        //{
        //    var inputBytes = inputHex.FromHexString();

        //    return Hash.Sha256(inputBytes).ToHexString();
        //}

        public static string DoubleSha256(byte[] input)
        {
            var hash = SHA256.HashData(input);
            var doubleHash = SHA256.HashData(hash);
            return doubleHash.ToHexFromByteArray();
        }

        public static string DoubleSha256(string input)
        {
            var inputBytes = Encoding.Default.GetBytes(input);

            return Hash.DoubleSha256(inputBytes);
        }

        public static byte[] Ripemd160(string input)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(input);
            var result = Ripemd160Lazy.Value.ComputeHash(bytes);
            return result;
        }

        public static byte[] Ripemd160Hex(string inputHex)
        {
            var bytes = inputHex.ToByteArrayFromHex();
            var result = Ripemd160Lazy.Value.ComputeHash(bytes);
            return result;
        }
    }
}
