﻿
// Copyright (c) Harry Pierson. All rights reserved.
// Licensed under the MIT license. 
// See LICENSE file in the project root for full license information.

using System;

namespace Terra.Net.Crypto.Ripemd160
{
    public sealed partial class Ripemd160
    {
        public sealed class IncrementalHash
        {
            private readonly Ripemd160HashProvider hashProvider;

            public const int HashSize = Ripemd160HashProvider.RequiredBufferLength;

            public IncrementalHash()
            {
                hashProvider = new Ripemd160HashProvider();
            }

            public void AppendData(ReadOnlySpan<byte> data)
            {
                hashProvider.AppendHashData(data);
            }

            public void AppendData(byte[] data)
            {
                if (data == null)
                {
                    throw new ArgumentNullException(nameof(data));
                }

                AppendData(data.AsSpan());
            }

            public void AppendData(byte[] data, int offset, int count)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));
                if (offset < 0)
                    throw new ArgumentOutOfRangeException(nameof(offset));
                if (count < 0 || (count > data.Length))
                    throw new ArgumentOutOfRangeException(nameof(count));
                if ((data.Length - count) < offset)
                    throw new ArgumentException();

                AppendData(data.AsSpan(offset, count));
            }

            public bool TryGetHashAndReset(Span<byte> destination, out int bytesWritten)
            {
                return hashProvider.TryFinalizeHashAndReset(destination, out bytesWritten);
            }

            public byte[] GetHashAndReset()
            {
                return hashProvider.FinalizeHashAndReset();
            }
        }
    }
}