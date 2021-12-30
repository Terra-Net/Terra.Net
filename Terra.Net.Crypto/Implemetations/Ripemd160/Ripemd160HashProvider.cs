﻿// Copyright (c) Harry Pierson. All rights reserved.
// Licensed under the MIT license. 
// See LICENSE file in the project root for full license information.

// Core RIPEMD-160 implementation originally ported to C# and 
// contributed to .NET Foundation by Darren R. Starr 
// https://github.com/darrenstarr/RIPEMD160.net

using System;
using System.Buffers.Binary;
using System.Diagnostics;

namespace Terra.Net.Crypto.Ripemd160
{

    // This class is modeled after the CoreFx internal HashProvider class. 
    // Breaking this out into a separate class allows the core hash logic 
    // to be shared between the HashAlgorithm and IncrementalHash implementations
    // of RIPEMD-160

    internal class Ripemd160HashProvider
    {
        private const int RMDsize = 160;
        public const int RequiredBufferLength = RMDsize / 8;
        private uint[] MDbuf = new uint[RMDsize / 32];
        private uint[] X = Array.Empty<uint>();               /* current 16-word chunk        */
        private readonly byte[] UnhashedBuffer = new byte[64];
        private int UnhashedBufferLength = 0;
        private long HashedLength = 0;

        public Ripemd160HashProvider()
        {
            ResetHashObject();
        }

        public void AppendHashData(ReadOnlySpan<byte> source)
        {
            var index = 0;
            var cbSize = source.Length;
            while (index < cbSize)
            {
                var bytesRemaining = cbSize - index;
                if (UnhashedBufferLength > 0)
                {
                    if ((bytesRemaining + UnhashedBufferLength) >= (UnhashedBuffer.Length))
                    {
                        var len = (UnhashedBuffer.Length) - UnhashedBufferLength;
                        source.Slice(index, len).CopyTo(UnhashedBuffer.AsSpan().Slice(UnhashedBufferLength, len));
                        index += (UnhashedBuffer.Length) - UnhashedBufferLength;
                        UnhashedBufferLength = UnhashedBuffer.Length;

                        for (var i = 0; i < 16; i++)
                            X[i] = BinaryPrimitives.ReadUInt32LittleEndian(UnhashedBuffer.AsSpan().Slice(i * 4));

                        compress(ref MDbuf, X);
                        UnhashedBufferLength = 0;
                    }
                    else
                    {
                        source.Slice(index, bytesRemaining).CopyTo(UnhashedBuffer.AsSpan().Slice(UnhashedBufferLength, bytesRemaining));
                        UnhashedBufferLength += bytesRemaining;
                        index += bytesRemaining;
                    }
                }
                else
                {
                    if (bytesRemaining >= (UnhashedBuffer.Length))
                    {
                        for (var i = 0; i < 16; i++)
                            X[i] = BinaryPrimitives.ReadUInt32LittleEndian(source.Slice(index + (i * 4)));
                        index += UnhashedBuffer.Length;

                        compress(ref MDbuf, X);
                    }
                    else
                    {
                        source.Slice(index, bytesRemaining).CopyTo(UnhashedBuffer);
                        UnhashedBufferLength = bytesRemaining;
                        index += bytesRemaining;
                    }
                }
            }

            HashedLength += cbSize;
        }

        public byte[] FinalizeHashAndReset()
        {
            var hash = new byte[RequiredBufferLength];
            bool success = TryFinalizeHashAndReset(hash, out int bytesWritten);
            Debug.Assert(success);
            Debug.Assert(hash.Length == bytesWritten);
            return hash;
        }

        public bool TryFinalizeHashAndReset(Span<byte> destination, out int bytesWritten)
        {
            if (destination.Length < RequiredBufferLength)
            {
                bytesWritten = default;
                return false;
            }

            if (HashedLength > uint.MaxValue)
            {
                throw new OverflowException();
            }

            MDfinish(ref MDbuf, UnhashedBuffer, 0, (uint)HashedLength, 0);

            for (var i = 0; i < RequiredBufferLength; i += 4)
            {
                destination[i] = (byte)(MDbuf[i >> 2] & 0xFF);         /* implicit cast to byte  */
                destination[i + 1] = (byte)((MDbuf[i >> 2] >> 8) & 0xFF);  /*  extracts the 8 least  */
                destination[i + 2] = (byte)((MDbuf[i >> 2] >> 16) & 0xFF);  /*  significant bits.     */
                destination[i + 3] = (byte)((MDbuf[i >> 2] >> 24) & 0xFF);
            }

            bytesWritten = RequiredBufferLength;
            ResetHashObject();
            return true;
        }

        private void ResetHashObject()
        {
            // initializes MDbuffer to "magic constants"
            MDbuf[0] = 0x67452301;
            MDbuf[1] = 0xefcdab89;
            MDbuf[2] = 0x98badcfe;
            MDbuf[3] = 0x10325476;
            MDbuf[4] = 0xc3d2e1f0;

            X = new uint[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            HashedLength = 0;
            UnhashedBufferLength = 0;
        }

        //  the compression function.
        //  transforms MDbuf using message bytes X[0] through X[15]
        static public void compress(ref uint[] MDbuf, uint[] X)
        {
            uint aa = MDbuf[0];
            uint bb = MDbuf[1];
            uint cc = MDbuf[2];
            uint dd = MDbuf[3];
            uint ee = MDbuf[4];
            uint aaa = MDbuf[0];
            uint bbb = MDbuf[1];
            uint ccc = MDbuf[2];
            uint ddd = MDbuf[3];
            uint eee = MDbuf[4];

            /* round 1 */
            FF(ref aa, bb, ref cc, dd, ee, X[0], 11);
            FF(ref ee, aa, ref bb, cc, dd, X[1], 14);
            FF(ref dd, ee, ref aa, bb, cc, X[2], 15);
            FF(ref cc, dd, ref ee, aa, bb, X[3], 12);
            FF(ref bb, cc, ref dd, ee, aa, X[4], 5);
            FF(ref aa, bb, ref cc, dd, ee, X[5], 8);
            FF(ref ee, aa, ref bb, cc, dd, X[6], 7);
            FF(ref dd, ee, ref aa, bb, cc, X[7], 9);
            FF(ref cc, dd, ref ee, aa, bb, X[8], 11);
            FF(ref bb, cc, ref dd, ee, aa, X[9], 13);
            FF(ref aa, bb, ref cc, dd, ee, X[10], 14);
            FF(ref ee, aa, ref bb, cc, dd, X[11], 15);
            FF(ref dd, ee, ref aa, bb, cc, X[12], 6);
            FF(ref cc, dd, ref ee, aa, bb, X[13], 7);
            FF(ref bb, cc, ref dd, ee, aa, X[14], 9);
            FF(ref aa, bb, ref cc, dd, ee, X[15], 8);

            /* round 2 */
            GG(ref ee, aa, ref bb, cc, dd, X[7], 7);
            GG(ref dd, ee, ref aa, bb, cc, X[4], 6);
            GG(ref cc, dd, ref ee, aa, bb, X[13], 8);
            GG(ref bb, cc, ref dd, ee, aa, X[1], 13);
            GG(ref aa, bb, ref cc, dd, ee, X[10], 11);
            GG(ref ee, aa, ref bb, cc, dd, X[6], 9);
            GG(ref dd, ee, ref aa, bb, cc, X[15], 7);
            GG(ref cc, dd, ref ee, aa, bb, X[3], 15);
            GG(ref bb, cc, ref dd, ee, aa, X[12], 7);
            GG(ref aa, bb, ref cc, dd, ee, X[0], 12);
            GG(ref ee, aa, ref bb, cc, dd, X[9], 15);
            GG(ref dd, ee, ref aa, bb, cc, X[5], 9);
            GG(ref cc, dd, ref ee, aa, bb, X[2], 11);
            GG(ref bb, cc, ref dd, ee, aa, X[14], 7);
            GG(ref aa, bb, ref cc, dd, ee, X[11], 13);
            GG(ref ee, aa, ref bb, cc, dd, X[8], 12);

            /* round 3 */
            HH(ref dd, ee, ref aa, bb, cc, X[3], 11);
            HH(ref cc, dd, ref ee, aa, bb, X[10], 13);
            HH(ref bb, cc, ref dd, ee, aa, X[14], 6);
            HH(ref aa, bb, ref cc, dd, ee, X[4], 7);
            HH(ref ee, aa, ref bb, cc, dd, X[9], 14);
            HH(ref dd, ee, ref aa, bb, cc, X[15], 9);
            HH(ref cc, dd, ref ee, aa, bb, X[8], 13);
            HH(ref bb, cc, ref dd, ee, aa, X[1], 15);
            HH(ref aa, bb, ref cc, dd, ee, X[2], 14);
            HH(ref ee, aa, ref bb, cc, dd, X[7], 8);
            HH(ref dd, ee, ref aa, bb, cc, X[0], 13);
            HH(ref cc, dd, ref ee, aa, bb, X[6], 6);
            HH(ref bb, cc, ref dd, ee, aa, X[13], 5);
            HH(ref aa, bb, ref cc, dd, ee, X[11], 12);
            HH(ref ee, aa, ref bb, cc, dd, X[5], 7);
            HH(ref dd, ee, ref aa, bb, cc, X[12], 5);

            /* round 4 */
            II(ref cc, dd, ref ee, aa, bb, X[1], 11);
            II(ref bb, cc, ref dd, ee, aa, X[9], 12);
            II(ref aa, bb, ref cc, dd, ee, X[11], 14);
            II(ref ee, aa, ref bb, cc, dd, X[10], 15);
            II(ref dd, ee, ref aa, bb, cc, X[0], 14);
            II(ref cc, dd, ref ee, aa, bb, X[8], 15);
            II(ref bb, cc, ref dd, ee, aa, X[12], 9);
            II(ref aa, bb, ref cc, dd, ee, X[4], 8);
            II(ref ee, aa, ref bb, cc, dd, X[13], 9);
            II(ref dd, ee, ref aa, bb, cc, X[3], 14);
            II(ref cc, dd, ref ee, aa, bb, X[7], 5);
            II(ref bb, cc, ref dd, ee, aa, X[15], 6);
            II(ref aa, bb, ref cc, dd, ee, X[14], 8);
            II(ref ee, aa, ref bb, cc, dd, X[5], 6);
            II(ref dd, ee, ref aa, bb, cc, X[6], 5);
            II(ref cc, dd, ref ee, aa, bb, X[2], 12);

            /* round 5 */
            JJ(ref bb, cc, ref dd, ee, aa, X[4], 9);
            JJ(ref aa, bb, ref cc, dd, ee, X[0], 15);
            JJ(ref ee, aa, ref bb, cc, dd, X[5], 5);
            JJ(ref dd, ee, ref aa, bb, cc, X[9], 11);
            JJ(ref cc, dd, ref ee, aa, bb, X[7], 6);
            JJ(ref bb, cc, ref dd, ee, aa, X[12], 8);
            JJ(ref aa, bb, ref cc, dd, ee, X[2], 13);
            JJ(ref ee, aa, ref bb, cc, dd, X[10], 12);
            JJ(ref dd, ee, ref aa, bb, cc, X[14], 5);
            JJ(ref cc, dd, ref ee, aa, bb, X[1], 12);
            JJ(ref bb, cc, ref dd, ee, aa, X[3], 13);
            JJ(ref aa, bb, ref cc, dd, ee, X[8], 14);
            JJ(ref ee, aa, ref bb, cc, dd, X[11], 11);
            JJ(ref dd, ee, ref aa, bb, cc, X[6], 8);
            JJ(ref cc, dd, ref ee, aa, bb, X[15], 5);
            JJ(ref bb, cc, ref dd, ee, aa, X[13], 6);

            /* parallel round 1 */
            JJJ(ref aaa, bbb, ref ccc, ddd, eee, X[5], 8);
            JJJ(ref eee, aaa, ref bbb, ccc, ddd, X[14], 9);
            JJJ(ref ddd, eee, ref aaa, bbb, ccc, X[7], 9);
            JJJ(ref ccc, ddd, ref eee, aaa, bbb, X[0], 11);
            JJJ(ref bbb, ccc, ref ddd, eee, aaa, X[9], 13);
            JJJ(ref aaa, bbb, ref ccc, ddd, eee, X[2], 15);
            JJJ(ref eee, aaa, ref bbb, ccc, ddd, X[11], 15);
            JJJ(ref ddd, eee, ref aaa, bbb, ccc, X[4], 5);
            JJJ(ref ccc, ddd, ref eee, aaa, bbb, X[13], 7);
            JJJ(ref bbb, ccc, ref ddd, eee, aaa, X[6], 7);
            JJJ(ref aaa, bbb, ref ccc, ddd, eee, X[15], 8);
            JJJ(ref eee, aaa, ref bbb, ccc, ddd, X[8], 11);
            JJJ(ref ddd, eee, ref aaa, bbb, ccc, X[1], 14);
            JJJ(ref ccc, ddd, ref eee, aaa, bbb, X[10], 14);
            JJJ(ref bbb, ccc, ref ddd, eee, aaa, X[3], 12);
            JJJ(ref aaa, bbb, ref ccc, ddd, eee, X[12], 6);

            /* parallel round 2 */
            III(ref eee, aaa, ref bbb, ccc, ddd, X[6], 9);
            III(ref ddd, eee, ref aaa, bbb, ccc, X[11], 13);
            III(ref ccc, ddd, ref eee, aaa, bbb, X[3], 15);
            III(ref bbb, ccc, ref ddd, eee, aaa, X[7], 7);
            III(ref aaa, bbb, ref ccc, ddd, eee, X[0], 12);
            III(ref eee, aaa, ref bbb, ccc, ddd, X[13], 8);
            III(ref ddd, eee, ref aaa, bbb, ccc, X[5], 9);
            III(ref ccc, ddd, ref eee, aaa, bbb, X[10], 11);
            III(ref bbb, ccc, ref ddd, eee, aaa, X[14], 7);
            III(ref aaa, bbb, ref ccc, ddd, eee, X[15], 7);
            III(ref eee, aaa, ref bbb, ccc, ddd, X[8], 12);
            III(ref ddd, eee, ref aaa, bbb, ccc, X[12], 7);
            III(ref ccc, ddd, ref eee, aaa, bbb, X[4], 6);
            III(ref bbb, ccc, ref ddd, eee, aaa, X[9], 15);
            III(ref aaa, bbb, ref ccc, ddd, eee, X[1], 13);
            III(ref eee, aaa, ref bbb, ccc, ddd, X[2], 11);

            /* parallel round 3 */
            HHH(ref ddd, eee, ref aaa, bbb, ccc, X[15], 9);
            HHH(ref ccc, ddd, ref eee, aaa, bbb, X[5], 7);
            HHH(ref bbb, ccc, ref ddd, eee, aaa, X[1], 15);
            HHH(ref aaa, bbb, ref ccc, ddd, eee, X[3], 11);
            HHH(ref eee, aaa, ref bbb, ccc, ddd, X[7], 8);
            HHH(ref ddd, eee, ref aaa, bbb, ccc, X[14], 6);
            HHH(ref ccc, ddd, ref eee, aaa, bbb, X[6], 6);
            HHH(ref bbb, ccc, ref ddd, eee, aaa, X[9], 14);
            HHH(ref aaa, bbb, ref ccc, ddd, eee, X[11], 12);
            HHH(ref eee, aaa, ref bbb, ccc, ddd, X[8], 13);
            HHH(ref ddd, eee, ref aaa, bbb, ccc, X[12], 5);
            HHH(ref ccc, ddd, ref eee, aaa, bbb, X[2], 14);
            HHH(ref bbb, ccc, ref ddd, eee, aaa, X[10], 13);
            HHH(ref aaa, bbb, ref ccc, ddd, eee, X[0], 13);
            HHH(ref eee, aaa, ref bbb, ccc, ddd, X[4], 7);
            HHH(ref ddd, eee, ref aaa, bbb, ccc, X[13], 5);

            /* parallel round 4 */
            GGG(ref ccc, ddd, ref eee, aaa, bbb, X[8], 15);
            GGG(ref bbb, ccc, ref ddd, eee, aaa, X[6], 5);
            GGG(ref aaa, bbb, ref ccc, ddd, eee, X[4], 8);
            GGG(ref eee, aaa, ref bbb, ccc, ddd, X[1], 11);
            GGG(ref ddd, eee, ref aaa, bbb, ccc, X[3], 14);
            GGG(ref ccc, ddd, ref eee, aaa, bbb, X[11], 14);
            GGG(ref bbb, ccc, ref ddd, eee, aaa, X[15], 6);
            GGG(ref aaa, bbb, ref ccc, ddd, eee, X[0], 14);
            GGG(ref eee, aaa, ref bbb, ccc, ddd, X[5], 6);
            GGG(ref ddd, eee, ref aaa, bbb, ccc, X[12], 9);
            GGG(ref ccc, ddd, ref eee, aaa, bbb, X[2], 12);
            GGG(ref bbb, ccc, ref ddd, eee, aaa, X[13], 9);
            GGG(ref aaa, bbb, ref ccc, ddd, eee, X[9], 12);
            GGG(ref eee, aaa, ref bbb, ccc, ddd, X[7], 5);
            GGG(ref ddd, eee, ref aaa, bbb, ccc, X[10], 15);
            GGG(ref ccc, ddd, ref eee, aaa, bbb, X[14], 8);

            /* parallel round 5 */
            FFF(ref bbb, ccc, ref ddd, eee, aaa, X[12], 8);
            FFF(ref aaa, bbb, ref ccc, ddd, eee, X[15], 5);
            FFF(ref eee, aaa, ref bbb, ccc, ddd, X[10], 12);
            FFF(ref ddd, eee, ref aaa, bbb, ccc, X[4], 9);
            FFF(ref ccc, ddd, ref eee, aaa, bbb, X[1], 12);
            FFF(ref bbb, ccc, ref ddd, eee, aaa, X[5], 5);
            FFF(ref aaa, bbb, ref ccc, ddd, eee, X[8], 14);
            FFF(ref eee, aaa, ref bbb, ccc, ddd, X[7], 6);
            FFF(ref ddd, eee, ref aaa, bbb, ccc, X[6], 8);
            FFF(ref ccc, ddd, ref eee, aaa, bbb, X[2], 13);
            FFF(ref bbb, ccc, ref ddd, eee, aaa, X[13], 6);
            FFF(ref aaa, bbb, ref ccc, ddd, eee, X[14], 5);
            FFF(ref eee, aaa, ref bbb, ccc, ddd, X[0], 15);
            FFF(ref ddd, eee, ref aaa, bbb, ccc, X[3], 13);
            FFF(ref ccc, ddd, ref eee, aaa, bbb, X[9], 11);
            FFF(ref bbb, ccc, ref ddd, eee, aaa, X[11], 11);

            // combine results */
            ddd += cc + MDbuf[1];               /* final result for MDbuf[0] */
            MDbuf[1] = MDbuf[2] + dd + eee;
            MDbuf[2] = MDbuf[3] + ee + aaa;
            MDbuf[3] = MDbuf[4] + aa + bbb;
            MDbuf[4] = MDbuf[0] + bb + ccc;
            MDbuf[0] = ddd;
        }

        //  puts bytes from strptr into X and pad out; appends length 
        //  and finally, compresses the last block(s)
        //  note: length in bits == 8 * (lswlen + 2^32 mswlen).
        //  note: there are (lswlen mod 64) bytes left in strptr.
        static public void MDfinish(ref uint[] MDbuf, byte[] strptr, long index, uint lswlen, uint mswlen)
        {
            //UInt32 i;                                 /* counter       */
            var X = new uint[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; /* message words */

            /* put bytes from strptr into X */
            for (var i = 0; i < (lswlen & 63); i++)
            {
                /* byte i goes into word X[i div 4] at pos.  8*(i mod 4)  */
                X[i >> 2] ^= Convert.ToUInt32(strptr[i + index]) << (8 * (i & 3));
            }

            /* append the bit m_n == 1 */
            X[(lswlen >> 2) & 15] ^= (uint)1 << Convert.ToInt32(8 * (lswlen & 3) + 7);

            if ((lswlen & 63) > 55)
            {
                /* length goes to next block */
                compress(ref MDbuf, X);
                X = new uint[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            }

            /* append length in bits*/
            X[14] = lswlen << 3;
            X[15] = (lswlen >> 29) | (mswlen << 3);
            compress(ref MDbuf, X);
        }

        private static uint RotateLeft(uint value, int bits)
        {
            return (value << bits) | (value >> (32 - bits));
        }

        #region basic functions F(), through H() and FF() through III() 

        /* the five basic functions F(), G() and H() */
        private static uint F(uint x, uint y, uint z)
        {
            return x ^ y ^ z;
        }

        private static uint G(uint x, uint y, uint z)
        {
            return (x & y) | (~x & z);
        }

        private static uint H(uint x, uint y, uint z)
        {
            return (x | ~y) ^ z;
        }

        private static uint I(uint x, uint y, uint z)
        {
            return (x & z) | (y & ~z);
        }

        private static uint J(uint x, uint y, uint z)
        {
            return x ^ (y | ~z);
        }

        /* the ten basic operations FF() through III() */

        private static void FF(ref uint a, uint b, ref uint c, uint d, uint e, uint x, int s)
        {
            a += F(b, c, d) + x;
            a = RotateLeft(a, s) + e;
            c = RotateLeft(c, 10);
        }


        private static void GG(ref uint a, uint b, ref uint c, uint d, uint e, uint x, int s)
        {
            a += G(b, c, d) + x + (uint)0x5a827999;
            a = RotateLeft(a, s) + e;
            c = RotateLeft(c, 10);
        }


        private static void HH(ref uint a, uint b, ref uint c, uint d, uint e, uint x, int s)
        {
            a += H(b, c, d) + x + (uint)0x6ed9eba1;
            a = RotateLeft(a, s) + e;
            c = RotateLeft(c, 10);
        }

        private static void II(ref uint a, uint b, ref uint c, uint d, uint e, uint x, int s)
        {
            a += I(b, c, d) + x + (uint)0x8f1bbcdc;
            a = RotateLeft(a, s) + e;
            c = RotateLeft(c, 10);
        }

        private static void JJ(ref uint a, uint b, ref uint c, uint d, uint e, uint x, int s)
        {
            a += J(b, c, d) + x + (uint)0xa953fd4e;
            a = RotateLeft(a, s) + e;
            c = RotateLeft(c, 10);
        }

        private static void FFF(ref uint a, uint b, ref uint c, uint d, uint e, uint x, int s)
        {
            a += F(b, c, d) + x;
            a = RotateLeft(a, s) + e;
            c = RotateLeft(c, 10);
        }

        private static void GGG(ref uint a, uint b, ref uint c, uint d, uint e, uint x, int s)
        {
            a += G(b, c, d) + x + (uint)0x7a6d76e9;
            a = RotateLeft(a, s) + e;
            c = RotateLeft(c, 10);
        }

        private static void HHH(ref uint a, uint b, ref uint c, uint d, uint e, uint x, int s)
        {
            a += H(b, c, d) + x + (uint)0x6d703ef3;
            a = RotateLeft(a, s) + e;
            c = RotateLeft(c, 10);
        }

        private static void III(ref uint a, uint b, ref uint c, uint d, uint e, uint x, int s)
        {
            a += I(b, c, d) + x + (uint)0x5c4dd124;
            a = RotateLeft(a, s) + e;
            c = RotateLeft(c, 10);
        }

        private static void JJJ(ref uint a, uint b, ref uint c, uint d, uint e, uint x, int s)
        {
            a += J(b, c, d) + x + (uint)0x50a28be6;
            a = RotateLeft(a, s) + e;
            c = RotateLeft(c, 10);
        }
        #endregion
    }
}
