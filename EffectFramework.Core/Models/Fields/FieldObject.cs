using EffectFramework.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace EffectFramework.Core.Models.Fields
{
    [Serializable]
    public class FieldObject<T> : FieldBase, IField, ISerializableField
    {
        // Must take great care in changing this to convert
        // existing data. Or put cases for each in serialize/
        // deserialize methods
        private readonly bool Compress = false;

        public FieldTypeMetaBasic MetaObject
        {
            get
            {
                return (FieldTypeMetaBasic)Meta;
            }
        }
        public override IFieldTypeMeta Meta
        {
            get
            {
                return base.Meta;
            }
            protected set
            {
                if (!(value is FieldTypeMetaBasic))
                {
                    Log.Error("Must assign a FieldTypeMetaBasic to an object field. Value type: {0}, Field ID: {1}", value?.GetType()?.Name, FieldID);
                    throw new InvalidCastException("Must assign a FieldTypeMetaBasic to a binary field.");
                }
                _Meta = (FieldTypeMetaBasic)value;
            }
        }

        private T _Value;
        private T _OriginalValue;
        public T Value
        {
            get
            {
                // We need to be very conservative to avoid
                // data loss. Therefore, any access of this
                // property triggers dirty.
                this.Dirty = true;
                return this._Value;
            }
            set
            {
                if (_Value == null && value != null ||
                    value == null && _Value != null ||
                    (_Value != null && !_Value.Equals(value)))
                {
                    this.Dirty = true;
                    this._Value = value;
                }
            }
        }

        protected override ValidationSummary ValidationHook()
        {
            List<ValidationResult> Errors = new List<ValidationResult>();
            if (_Value is IValidatable)
            {
                Errors.AddRange(((IValidatable)_Value).Validate().Errors);
            }
            return new ValidationSummary(Errors);
        }

        private byte[] Serialize(T Obj)
        {
            BinaryFormatter Formatter = new BinaryFormatter();
            byte[] Bytes;

            using (var Stream = new MemoryStream())
            {
                if (Compress)
                {
                    using (var GZipStream = new GZipStream(Stream, CompressionMode.Compress))
                    {
                        Formatter.Serialize(GZipStream, Obj);
                    }
                }
                else
                {
                    Formatter.Serialize(Stream, Obj);
                }
                Stream.Position = 0;
                Bytes = Stream.GetBuffer();
            }

            return Bytes;
        }

        private T Unserialize(byte[] Bytes)
        {
            BinaryFormatter Formatter = new BinaryFormatter();
            T Output;
            using (var Stream = new MemoryStream(Bytes))
            {
                if (Compress)
                {
                    using (var GZipStream = new GZipStream(Stream, CompressionMode.Decompress))
                    {
                        object Temp = Formatter.Deserialize(GZipStream);
                        if (!(Temp is T))
                        {
                            throw new InvalidCastException();
                        }
                        Output = (T)Temp;
                    }
                }
                else
                {
                    object Temp = Formatter.Deserialize(Stream);
                    if (!(Temp is T))
                    {
                        throw new InvalidCastException();
                    }
                    Output = (T)Temp;
                }
            }
            return Output;
        }

        public void Dirtify()
        {
            this.Dirty = true;
        }

        object IField.Value
        {
            get
            {
                // Need to be very conservative to avoid
                // data lossable.
                this.Dirty = true;
                return this._Value;
            }

            set
            {
                if (value != null && !(value is T))
                {
                    Log.Error("Must assign an object to an object field. Value Type: {0}, Field ID: {1}", value?.GetType()?.Name, FieldID);
                    throw new InvalidCastException("Must assign an object to a binary field.");
                }
                if ((_Value == null && value != null) || (_Value != null && value == null) || (_Value != null && !_Value.Equals(value)))
                {
                    this.Dirty = true;
                    this._Value = (T)value;
                }
            }
        }

        public object DereferencedValue
        {
            get
            {
                return ValueBinary;
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
                return this._OriginalValue;
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
            return _Value.Equals(Value);
        }

        public void SerializeField()
        {
            if (_Value != null)
            {
                this.ValueBinary = Serialize(_Value);
            }
            else
            {
                this.ValueBinary = null;
            }
        }

        public void DeserializeField()
        {
            if (this.ValueBinary != null)
            {
                _Value = Unserialize(this.ValueBinary);
            }
            else
            {
                _Value = default(T);
            }
        }

        public FieldObject()
            : base()
        { }

        public FieldObject(FieldType Type, EntityBase Entity)
            : this(Type, null, Entity)
        {

        }

        public FieldObject(FieldType Type)
            : this(Type, null, null)
        {

        }

        public FieldObject(FieldType Type, FieldBase Base, EntityBase Entity, Db.IDbContext ctx = null)
            : base(Type, Base, Entity, ctx)
        {
            if (Type.DataType != DataType.Object)
            {
                throw new ArgumentOutOfRangeException("Cannot create a binary field from a non-binary type.");
            }

            if (this.ValueBinary != null)
            {
                this._Value = Unserialize(this.ValueBinary);
            }

            if (this.OriginalValueBinary != null)
            {
                this._OriginalValue = Unserialize(this.OriginalValueBinary);
            }
        }
    }
}
