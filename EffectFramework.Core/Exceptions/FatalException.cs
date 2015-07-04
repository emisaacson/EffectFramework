using System;

namespace EffectFramework.Core.Exceptions
{
    public class FatalException : Exception
    {
        public FatalException() : base() { }
        public FatalException(string Message) : base(Message) { }
    }
}
