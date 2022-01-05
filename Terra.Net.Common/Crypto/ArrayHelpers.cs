using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terra.Net.Crypto
{
    public static class ArrayHelpers
    {
        public static byte[] Concat(byte[] arr, params byte[][] arrs)
        {
            var len = arr.Length + arrs.Sum(a => a.Length);
            var ret = new byte[len];
            Buffer.BlockCopy(arr, 0, ret, 0, arr.Length);
            var pos = arr.Length;
            foreach (var a in arrs)
            {
                Buffer.BlockCopy(a, 0, ret, pos, a.Length);
                pos += a.Length;
            }
            return ret;
        }


        public static byte[] ToByteArrayFromString(this string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }



        public static string ToStringFromByteArray(this byte[] data)
        {
            return System.Text.Encoding.UTF8.GetString(data, 0, data.Length);
        }
        public static byte[] ToByteArrayFromHex(this string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

        public static string ToHexFromByteArray(this byte[] data)
        {
            if (data == null)
            {
                return String.Empty;
            }

            string hex = BitConverter.ToString(data);
            return hex.Replace("-", "").ToLower();
        }
    }
}
