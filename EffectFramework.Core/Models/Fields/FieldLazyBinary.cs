using EffectFramework.Core.Models.Entities;
using System;

namespace EffectFramework.Core.Models.Fields
{
    public class FieldLazyBinary : FieldBinary, IField
    {
        public override bool IsLazy
        {
            get
            {
                return true;
            }
        }

        public FieldLazyBinary()
            : base()
        { }

        public FieldLazyBinary(FieldType Type, EntityBase Entity)
            : this(Type, null, Entity)
        {

        }

        public FieldLazyBinary(FieldType Type)
            : this(Type, null, null)
        {

        }

        public FieldLazyBinary(FieldType Type, FieldBase Base, EntityBase Entity)
            : base(Type, Base, Entity)
        {
            if (Type.DataType != DataType.Binary)
            {
                throw new ArgumentOutOfRangeException("Cannot create a binary field from a non-binary type.");
            }
        }
    }
}
