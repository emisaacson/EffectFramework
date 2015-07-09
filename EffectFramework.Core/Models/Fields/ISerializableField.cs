using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Core.Models.Fields
{
    /// <summary>
    /// Implement this interface if the field data needs to be
    /// serialized before being persisted.
    /// </summary>
    public interface ISerializableField
    {
        void SerializeField();
        void DeserializeField();
    }
}
