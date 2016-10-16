using System;
using System.Collections.Generic;
using System.Threading;

namespace EffectFramework.Core.Models.Fields
{
    /// <summary>
    /// The base class all fields must inherit from.
    /// </summary>
    [Serializable]
    public class FieldType
    {
        [NonSerialized]
        private static Logger _Log;
        private static object LogLock = new object();
        private static Logger Log
        {
            get
            {
                lock (LogLock)
                {
                    if (_Log == null)
                    {
                        _Log = new Logger(nameof(FieldType));
                    }
                    return _Log;
                }
            }
        }
        /// <summary>
        /// Gets or sets the field ID to match the persistence service..
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public long Value { get; protected set; }
        public DataType DataType { get; protected set; }
        public string Name { get; protected set; }
        public long? LookupTypeID { get; protected set; }
        public virtual long TenantID {
            get
            {
                return Configure.GetTenantResolutionProvider().GetTenantID();
            }
        }

        private static Dictionary<long, FieldType> TypeRegistry = new Dictionary<long, FieldType>();
        private static ReaderWriterLockSlim RegistryLock = new ReaderWriterLockSlim();

        protected FieldType(string Name, long Value, DataType DataType, long? LookupTypeID = null)
        {
            this.Name = Name;
            this.Value = Value;
            this.DataType = DataType;
            this.LookupTypeID = LookupTypeID;
            RegisterType(this);
        }

        public static implicit operator long (FieldType dt)
        {
            return dt.Value;
        }

        public static explicit operator FieldType(long i)
        {
            try
            {
                RegistryLock.EnterReadLock();
                if (TypeRegistry.ContainsKey(i))
                {
                    return TypeRegistry[i];
                }
            }
            finally
            {
                try
                {
                    RegistryLock.ExitReadLock();
                }
                catch (Exception e)
                {
                    Log.Error("Hell froze over.", e);
                }
            }
            throw new InvalidCastException(string.Format("Cannot convert the long value {0} to a FieldType instance.", i));
        }

        private static void RegisterType(FieldType Type)
        {
            try
            {
                RegistryLock.EnterWriteLock();
                if (TypeRegistry.ContainsKey(Type.Value))
                {
                    throw new InvalidOperationException("Cannot register the same Field Type twice.");
                }
                TypeRegistry[Type.Value] = Type;
            }
            finally
            {
                try
                {
                    RegistryLock.ExitWriteLock();
                }
                catch (Exception e)
                {
                    Log.Error("Hell froze over.", e);
                }
            }
        }

        public static bool operator ==(FieldType Ft1, FieldType Ft2)
        {
            if ((object)Ft1 == null && (object)Ft2 == null)
            {
                return true;
            }
            if ((object)Ft1 == null || (object)Ft2 == null)
            {
                return false;
            }

            return Ft1.Value == Ft2.Value;
        }

        public static bool operator !=(FieldType Ft1, FieldType Ft2)
        {
            if ((object)Ft1 == null && (object)Ft2 == null)
            {
                return false;
            }
            if ((object)Ft1 == null || (object)Ft2 == null)
            {
                return true;
            }

            return Ft1.Value != Ft2.Value;
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
