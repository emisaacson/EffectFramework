using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Exceptions
{
    public class FatalException : Exception
    {
        public FatalException() : base() { }
        public FatalException(string Message) : base(Message) { }
    }
}
