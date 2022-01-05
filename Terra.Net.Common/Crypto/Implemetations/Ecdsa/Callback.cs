using System;

namespace Terra.Net.Crypto.Ecdsa
{
    public class Callback : EventArgs
    {
        public Callback()
        {
        }

        public Callback(string message)
        {
            Message = message;
        }

        public string Message;
    }
}