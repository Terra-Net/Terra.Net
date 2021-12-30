using System;
using System.Security.Cryptography;

namespace Terra.Net.Crypto.Ripemd160
    {
        public sealed partial class Ripemd160 : HashAlgorithm
        {
            public new const int HashSize = Ripemd160HashProvider.RequiredBufferLength;

            private readonly Ripemd160HashProvider hashProvider;

            public Ripemd160()
            {
                hashProvider = new Ripemd160HashProvider();
                HashSizeValue = Ripemd160HashProvider.RequiredBufferLength;
            }

            public override void Initialize()
            {
            }

            protected override void HashCore(byte[] array, int ibStart, int cbSize)
            {
                hashProvider.AppendHashData(array.AsSpan(ibStart, cbSize));
            }

            protected override void HashCore(ReadOnlySpan<byte> source)
            {
                hashProvider.AppendHashData(source);
            }

            protected override byte[] HashFinal()
            {
                return hashProvider.FinalizeHashAndReset();
            }

            protected override bool TryHashFinal(Span<byte> destination, out int bytesWritten)
            {
                return hashProvider.TryFinalizeHashAndReset(destination, out bytesWritten);
            }
        }
    }
