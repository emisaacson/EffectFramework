using System;
using EffectFramework.Core.Services;
using System.Linq;

namespace EffectFramework.Core.Models.Fields
{
    public class FieldBinary : FieldBase, IField
    {
        public string Name { get; private set; }
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

        object IField.Value
        {
            get
            {
                return this.ValueBinary;
            }

            set
            {
                if (!typeof(byte[]).IsAssignableFrom(value.GetType()))
                {
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

        public FieldBinary(IPersistenceService PersistenceService)
            : base(PersistenceService)
        { }

        public FieldBinary(FieldType Type, IPersistenceService PersistenceService)
            : this(Type, null, PersistenceService)
        {

        }

        public FieldBinary(FieldType Type, FieldBase Base, IPersistenceService PersistenceService)
            : base(PersistenceService)
        {
            if (Type.DataType != DataType.Binary)
            {
                throw new ArgumentOutOfRangeException("Cannot create a binary field from a non-binary type.");
            }
            this.Type = Type;
            this.Name = Type.Name;

            if (Base != null)
            {
                LoadUpValues(Base);
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

                throw new ArgumentNullException();
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
