using EffectFramework.Core.Models.Entities;
using System;

namespace EffectFramework.Core.Models.Fields
{
    /// <summary>
    /// Each DataType gets an ID and an instance on this class
    /// </summary>
    [Serializable]
    public class DataType
    {
        [NonSerialized]
        private Logger _Log;
        private Logger Log
        {
            get
            {
                if (_Log == null)
                {
                    _Log = new Logger(GetType().Name);
                }
                return _Log;
            }
        }

        public int Value { get; private set; }
        public Type ValueType { get; private set; }
        public Type DereferencedType { get; private set; }
        public Type MetaType { get; private set; }

        private DataType(int Value, Type ValueType, Type DereferencedType, Type MetaType = null)
        {
            this.Value = Value;
            this.ValueType = ValueType;
            this.DereferencedType = DereferencedType;
            if (MetaType == null)
            {
                this.MetaType = typeof(FieldTypeMetaBasic);
            }
            else
            {
                if (!typeof(FieldTypeMetaBase).IsAssignableFrom(MetaType) || typeof(FieldTypeMetaBase) == MetaType)
                {
                    throw new ArgumentOutOfRangeException("MetaType must be a subclass of FieldTypeMetaBase.");
                }
                this.MetaType = MetaType;
            }
        }

        public static readonly DataType Text = new DataType(1, typeof(string), typeof(string), typeof(FieldTypeMetaText));
        public static readonly DataType Date = new DataType(2, typeof(DateTime?), typeof(DateTime?), typeof(FieldTypeMetaDate));
        public static readonly DataType Decimal = new DataType(3, typeof(decimal?), typeof(decimal?), typeof(FieldTypeMetaDecimal));
        public static readonly DataType Boolean = new DataType(4, typeof(bool?), typeof(bool?));
        public static readonly DataType Lookup = new DataType(5, typeof(int?), typeof(string));
        public static readonly DataType Binary = new DataType(6, typeof(byte[]), typeof(byte[]));
        public static readonly DataType ItemReference = new DataType(7, typeof(int?), typeof(Item));
        public static readonly DataType EntityReference = new DataType(8, typeof(int?), typeof(EntityBase));

        public static implicit operator int (DataType dt)
        {
            return dt.Value;
        }

        public static explicit operator DataType(int i)
        {
            switch (i)
            {
                case 1:
                    return Text;
                case 2:
                    return Date;
                case 3:
                    return Decimal;
                case 4:
                    return Boolean;
                case 5:
                    return Lookup;
                case 6:
                    return Binary;
                case 7:
                    return ItemReference;
                case 8:
                    return EntityReference;
                default:
                    throw new InvalidCastException(string.Format("Cannot convert the int value {0} to a DataType instance.", i));
            }
        }

        public static bool operator ==(DataType Dt1, DataType Dt2)
        {
            if ((object)Dt1 == null && (object)Dt2 == null)
            {
                return true;
            }
            if ((object)Dt1 == null || (object)Dt2 == null)
            {
                return false;
            }

            return Dt1.Value == Dt2.Value;
        }

        public static bool operator !=(DataType Dt1, DataType Dt2)
        {
            if ((object)Dt1 == null && (object)Dt2 == null)
            {
                return false;
            }
            if ((object)Dt1 == null || (object)Dt2 == null)
            {
                return true;
            }

            return Dt1.Value != Dt2.Value;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
