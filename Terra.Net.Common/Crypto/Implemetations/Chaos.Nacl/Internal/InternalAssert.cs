using System;

namespace Terra.Net.Crypto.Chaos.Nacl.Internal
{
    internal static class InternalAssert
    {
        public static void Assert(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException("An assertion in Chaos.Crypto failed " + message);
        }
    }
}
