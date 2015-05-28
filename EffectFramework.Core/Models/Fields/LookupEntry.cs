using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Models.Fields
{
    public class LookupEntry
    {
        public int ID { get; private set; }
        public string Value { get; private set; }

        public LookupEntry(int ID, string Value)
        {
            this.ID = ID;
            this.Value = Value;
        }
    }
}
