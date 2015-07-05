using System;
using System.Linq;
using EffectFramework.Core.Models.Entities;

namespace EffectFramework.Core.Models.Fields
{
    [Serializable]
    public class FieldBinary : FieldBase, IField
    {
        public byte[] Value
        {
            get
            {
                return this.ValueBinary;
            }
            set
            {
                if (this.ValueBinary != null && value == null ||
                    this.ValueBinary == null && value != null ||
                   (this.ValueBinary != null && !this.ValueBinary.SequenceEqual(value)))
                {
                    this.Dirty = true;
                    this.ValueBinary = value;
                }
            }
        }

        public FieldTypeMetaBasic MetaBinary
        {
            get
            {
                return (FieldTypeMetaBasic)Meta;
            }
        }
        public override IFieldTypeMeta Meta {
            get
            {
                return base.Meta;
            }
            protected set
            {
                if (!(value is FieldTypeMetaBasic))
                {
                    Log.Error("Must assign a FieldTypeMetaBasic to a binary field. Value type: {0}, Field ID: {1}", value?.GetType()?.Name, FieldID);
                    throw new InvalidCastException("Must assign a FieldTypeMetaBasic to a binary field.");
                }
                _Meta = (FieldTypeMetaBasic)value;
            }
        }

        object IField.Value
        {
            get
            {
                return this.ValueBinary;
            }

            set
            {
                if (value != null && !(value is byte[]))
                {
                    Log.Error("Must assign a byte array to a binary field. Value Type: {0}, Field ID: {1}", value?.GetType()?.Name, FieldID);
                    throw new InvalidCastException("Must assign a byte array to a binary field.");
                }
                if (this.ValueBinary != null && (byte[])value == null ||
                    this.ValueBinary == null && (byte[])value != null ||
                    (this.ValueBinary != null && !this.ValueBinary.SequenceEqual((byte[])value)))
                {
                    this.Dirty = true;
                    this.ValueBinary = (byte[])value;
                }
            }
        }

        public object DereferencedValue
        {
            get
            {
                return Value;
            }
        }

        public byte[] OriginalValue
        {
            get
            {
                return this.OriginalValueBinary;
            }
        }

        object IField.OriginalValue
        {
            get
            {
                return this.OriginalValueBinary;
            }
        }

        public object OriginalDereferencedValue
        {
            get
            {
                return this.OriginalValueBinary;
            }
        }

        public bool ValueEquals(object Value)
        {
            if (Value == null)
            {
                return this.Value == null;
            }
            else if (Value is string)
            {
                byte[] Decoded = Convert.FromBase64String((string)Value);
                return this.Value.SequenceEqual(Decoded);
            }
            else if (Value is byte[])
            {
                return this.Value.SequenceEqual((byte[])Value);
            }
            return false;
        }

        public FieldBinary()
            : base()
        { }

        public FieldBinary(FieldType Type, EntityBase Entity)
            : this(Type, null, Entity)
        {

        }

        public FieldBinary(FieldType Type)
            : this(Type, null, null)
        {

        }

        public FieldBinary(FieldType Type, FieldBase Base, EntityBase Entity, Db.IDbContext ctx = null)
            : base(Type, Base, Entity, ctx)
        {
            if (Type.DataType != DataType.Binary)
            {
                throw new ArgumentOutOfRangeException("Cannot create a binary field from a non-binary type.");
            }
        }


        /// <summary>
        /// Compares the value of this binary field to another and return true if they are identical, byte for byte.
        /// </summary>
        /// <param name="OtherField">The other field.</param>
        /// <returns>true if the field values are identical, false otherwise</returns>
        public override bool IsIdenticalTo(FieldBase OtherField)
        {
            if (OtherField == null)
            {
                Log.Warn("Trying to compare Field to a null Field. FieldID: {0}",
                        FieldID.HasValue ? FieldID.Value.ToString() : "null");

                throw new ArgumentNullException(nameof(OtherField));
            }
            if (OtherField.Type.DataType != this.Type.DataType)
            {
                Log.Warn("Trying to compare Field to a Field of a different type. FieldID: {0}, Other FieldID: {1}, Field Type: {2}, Other Field Type: {3}",
                    FieldID.HasValue ? FieldID.Value.ToString() : "null",
                    OtherField.FieldID.HasValue ? OtherField.FieldID.Value.ToString() : "null",
                    Type.Name, OtherField.Type.Name);

                throw new InvalidOperationException("Cannot compare two fields of different types.");
            }


            if (((IField)this).Value == null && ((IField)OtherField).Value == null) // Both are null, identical
            {
                return true;
            }

            if ((((IField)this).Value == null && ((IField)OtherField).Value != null) ||  // If a is null and b is not, not identical
                 (((IField)this).Value != null && ((IField)OtherField).Value == null))   // If b is null and a is not, not identical
            {
                return false;
            }

            // Both are not null, use byte-by-byte comparison.
            return this.Value.SequenceEqual(((FieldBinary)OtherField).Value);
        }
    }
}
