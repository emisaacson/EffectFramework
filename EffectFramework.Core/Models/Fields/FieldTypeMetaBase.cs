using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using EffectFramework.Core.Models.Db;

namespace EffectFramework.Core.Models.Fields
{
    /// <summary>
    /// Base class for all FieldTypeMeta classes
    /// </summary>
    [Serializable]
    public class FieldTypeMetaBase
    {
        [NonSerialized]
        protected Logger _Log;
        protected Logger Log
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

        [NonSerialized]
        private FieldBase _Field;
        public FieldBase Field
        {
            get
            {
                return _Field;
            }
            set
            {
                _Field = value;
                if (IsRequiredQuery != null)
                {
                    IsRequiredQuery.Item = _Field?.Entity?.Item;
                }
            }
        }

        protected bool? __IsRequired;
        protected bool? _IsRequired
        {
            get
            {
                if (__IsRequired.HasValue)
                {
                    return __IsRequired.Value;
                }
                else if (IsRequiredQuery != null)
                {
                    try
                    {
                        return IsRequiredQuery.ItemMatches();
                    }
                    catch (Exception e)
                    {
                        Log.Error("Could not run item match.", e);
                        return false;
                    }
                }
                return false;
            }
            set
            {
                __IsRequired = value;
            }
        }

        public bool IsRequired
        {
            get
            {
                var Req = _IsRequired;
                return Req.HasValue ? Req.Value : false;
            }
        }

        public IObjectQueryProvider IsRequiredQuery { get; protected set; }

        public Regex TextRegex { get; protected set; }
        // EITODO
        //public IObjectQueryProvider TextRegexQuery { get; protected set; }

        protected decimal? DecimalMin { get; set; }
        // EITODO
        //protected IObjectQueryProvider DecimalMinQuery { get; set }

        protected decimal? DecimalMax { get; set; }
        // EITODO
        //protected IObjectQueryProvider DecimalMaxQuery { get; set }

        protected DateTime? DateTimeMin { get; set; }
        // EITODO
        //protected IObjectQueryProvider DateTimeMinQuery { get; set }

        protected DateTime? DateTimeMax { get; set; }
        // EITODO
        //protected IObjectQueryProvider DateTimeMaxQuery { get; set }

        public virtual bool HasRange {
            get {
                return false;
            }
        }
        public virtual bool HasRegex
        {
            get
            {
                return false;
            }
        }

        public FieldTypeMetaBase() {
            this._IsRequired = false;
        }

        public FieldTypeMetaBase(FieldTypeMeta DbFieldTypeMeta)
        {
            if (DbFieldTypeMeta != null)
            {
                // EITODO: Read queries and evaluate them
                this._IsRequired = DbFieldTypeMeta.IsRequired;
                this.TextRegex = DbFieldTypeMeta.TextRegex != null ? new Regex(DbFieldTypeMeta.TextRegex) : null;
                this.DecimalMin = DbFieldTypeMeta.DecimalMin;
                this.DecimalMax = DbFieldTypeMeta.DecimalMax;
                this.DateTimeMin = DbFieldTypeMeta.DatetimeMin;
                this.DateTimeMax = DbFieldTypeMeta.DatetimeMax;

                if (DbFieldTypeMeta.IsRequiredQuery != null)
                {
                    this.IsRequiredQuery = Configure.GetObjectQueryProvider();
                    this.IsRequiredQuery.QueryText = DbFieldTypeMeta.IsRequiredQuery;
                }
            }
            else
            {
                this._IsRequired = false;
            }
        }

        internal void CopyValuesFrom(FieldTypeMetaBase OtherMeta)
        {
            if (OtherMeta == null)
            {
                throw new ArgumentNullException(nameof(OtherMeta));
            }
            this.__IsRequired = OtherMeta.__IsRequired;
            this.TextRegex = OtherMeta.TextRegex;
            this.DecimalMin = OtherMeta.DecimalMin;
            this.DecimalMax = OtherMeta.DecimalMax;
            this.DateTimeMax = OtherMeta.DateTimeMax;
            this.DateTimeMin = OtherMeta.DateTimeMin;

            if (OtherMeta.IsRequiredQuery != null)
            {
                this.IsRequiredQuery = Configure.GetObjectQueryProvider();
                this.IsRequiredQuery.QueryText = OtherMeta.IsRequiredQuery.QueryText;
            }
        }
    }
}
