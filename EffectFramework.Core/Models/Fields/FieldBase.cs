using System;
using EffectFramework.Core.Models.Entities;
using EffectFramework.Core.Services;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using EffectFramework.Core.Models.Db;
using System.Collections.Generic;
using EffectFramework.Core.Exceptions;

namespace EffectFramework.Core.Models.Fields
{
    /// <summary>
    /// The base class of all Fields
    /// </summary>
    [Serializable]
    public class FieldBase
    {
        [NonSerialized]
        protected Logger _Log;
        protected Logger Log {
            get
            {
                if (_Log == null)
                {
                    _Log = new Logger(GetType().Name);
                }
                return _Log;
            }
        }
        /// <summary>
        /// Gets the type of the Field instance
        /// </summary>
        /// <value>
        /// The static FieldType of this Field
        /// </value>
        public FieldType Type { get; protected set; }

        public virtual bool IsLazy
        {
            get
            {
                return false;
            }
        }

        protected IFieldTypeMeta _Meta;
        protected IFieldTypeMeta _DefaultMeta;
        protected virtual IFieldTypeMeta DefaultMeta
        {
            get
            {
                return new FieldTypeMetaBasic(false);
            }
        }
        public virtual IFieldTypeMeta Meta
        {
            get
            {
                if (_Meta == null)
                {
                    TryLoadFieldMeta();
                    if (_Meta /*still!*/ == null)
                    {
                        if (_DefaultMeta == null)
                        {
                            _DefaultMeta = DefaultMeta;
                        }

                        // The field does not get cached so it's very hard to tell
                        // if this will be filled in or not when we retreive it.
                        // Just set the field every time.
                        ((FieldTypeMetaBase)_DefaultMeta).Field = this;
                        return _DefaultMeta;
                    }
                    
                }
                // The field does not get cached so it's very hard to tell
                // if this will be filled in or not when we retreive it.
                // Just set the field every time.
                ((FieldTypeMetaBase)_Meta).Field = this;
                return _Meta;
            }
            protected set
            {
                _Meta = value;
            }
        }
        public EntityBase Entity { get; protected set; }
        public Guid Guid { get; protected set; }
        public int? FieldID { get; protected set; }
        public bool Dirty { get; protected set; }
        public string Name { get; protected set; }
        public virtual int TenantID { get; protected set; }

        protected string ValueString { get; set; }
        protected DateTime? ValueDate { get; set; }
        protected decimal? ValueDecimal { get; set; }
        protected bool? ValueBool { get; set; }
        protected int? ValueLookup { get; set; }
        protected int? ValueEntityReference { get; set; }
        protected int? ValueItemReference { get; set; }
        protected byte[] ValueBinary { get; set; }

        protected string OriginalValueString { get; set; }
        protected DateTime? OriginalValueDate { get; set; }
        protected decimal? OriginalValueDecimal { get; set; }
        protected bool? OriginalValueBool { get; set; }
        protected int? OriginalValueLookup { get; set; }
        protected int? OriginalValueEntityReference { get; set; }
        protected int? OriginalValueItemReference { get; set; }
        protected byte[] OriginalValueBinary { get; set; }

        [NonSerialized]
        protected IPersistenceService _PersistenceService;
        protected IPersistenceService PersistenceService
        {
            get
            {
                if (_PersistenceService == null)
                {
                    _PersistenceService = Configure.GetPersistenceService();
                }
                return _PersistenceService;
            }
        }
        [NonSerialized]
        protected ICacheService _CacheService;
        protected ICacheService CacheService
        {
            get
            {
                if (_CacheService == null)
                {
                    _CacheService = Configure.GetCacheService();
                }
                return _CacheService;
            }
        }

        public FieldBase()
        {
            this.Dirty = false;
            this.TenantID = Configure.GetTenantResolutionProvider().GetTenantID();
        }

        public FieldBase(FieldType Type, FieldBase Base)
        {
            this.Type = Type;
            this.Name = Type.Name;
            this.TenantID = Configure.GetTenantResolutionProvider().GetTenantID();
            LoadUpValues(Base);
        }

        public FieldBase(FieldType Type, FieldBase Base, EntityBase Entity)
        {
            this.Type = Type;
            this.Name = Type.Name;
            this.Entity = Entity;
            this.TenantID = Configure.GetTenantResolutionProvider().GetTenantID();
            LoadUpValues(Base);
            TryLoadFieldMeta();
        }

        public FieldBase(Db.Field Field)
        {
            this.TenantID = Configure.GetTenantResolutionProvider().GetTenantID();

            if (this.TenantID != Field.TenantID)
            {
                Log.Fatal("TenantID Does not match. FieldID: {0}, Global TenantID: {1}, Field TenantID: {2}",
                    Field.FieldID, this.TenantID, Field.TenantID);
                throw new FatalException("Data error.");
            }

            this.Dirty = false;
            this.FieldID = Field.FieldID;

            this.ValueString = Field.ValueText;
            this.ValueDate = Field.ValueDate;
            this.ValueDecimal = Field.ValueDecimal;
            this.ValueBool = Field.ValueBoolean;
            this.ValueLookup = Field.ValueLookup;
            this.ValueBinary = Field.ValueBinary;
            this.ValueEntityReference = Field.ValueEntityReference;
            this.ValueItemReference = Field.ValueItemReference;
            this.Guid = Field.Guid;

            RefreshOriginalValues();

        }

        protected void TryLoadFieldMeta()
        {
            if (Entity == null || Entity.Item == null)
            {
                Log.Trace("Cannot get field meta if Entity and Item not wired. FieldID: {0}", FieldID);
                return;
            }

            string FieldTypeMetaKey = string.Format("FieldTypeMeta:{0}:{1}:{2}", Entity.Item.Type.Value, Entity.Type.Value, Type.Value);

            IFieldTypeMeta RawMeta = (IFieldTypeMeta)CacheService.GetObject(FieldTypeMetaKey);
            IFieldTypeMeta MetaToUse;
            BinaryFormatter Formatter = new BinaryFormatter();
            if (RawMeta == null)
            {
                Log.Trace("Cache returned null FieldMetaData. Getting from Database. Cache Key: {0}", FieldTypeMetaKey);
                var FieldMeta = PersistenceService.GetFieldTypeMeta(Entity.Item.Type.Value, Entity.Type.Value, Type.Value);
                CacheService.StoreObject(FieldTypeMetaKey, FieldMeta);

                MetaToUse = FieldMeta;
            }
            else
            {
                Log.Trace("Cache returned FieldMetaData. Cache Key: {0}", FieldTypeMetaKey);
                MetaToUse = RawMeta;
            }

            if (((FieldTypeMetaBase)MetaToUse).TenantID != this.TenantID)
            {
                Log.Fatal("TenantID Does not match. FieldID: {0}, FieldTypeMetaID: {1}, Field TenantID: {2}, FieldTypeMeta TenantID : {3}",
                    this.FieldID, ((FieldTypeMetaBase)MetaToUse).FieldTypeMetaID, this.TenantID, ((FieldTypeMetaBase)MetaToUse).TenantID);
                throw new FatalException("Data error.");
            }

            Meta = MetaToUse;
        }

        protected void LoadUpValues(FieldBase Base)
        {
            Log.Trace("Loading values for Field. FieldID: {0}",
                Base == null || !Base.FieldID.HasValue ? "null" : Base.FieldID.Value.ToString());

            this.TenantID = Configure.GetTenantResolutionProvider().GetTenantID();
            if (Base == null)
            {
                this.FieldID      = null;

                this.ValueString  = null;
                this.ValueDate    = null;
                this.ValueDecimal = null;
                this.ValueBool    = null;
                this.ValueLookup  = null;
                this.ValueBinary  = null;
                this.ValueEntityReference = null;
                this.ValueItemReference = null;
            }
            else
            {
                if (Base.TenantID != this.TenantID)
                {
                    Log.Fatal("TenantID Does not match. FieldID: {0}, Global TenantID: {1}, Other Field TenantID : {2}",
                        this.FieldID, this.TenantID, Base.TenantID);
                    throw new FatalException("Data error.");
                }

                this.FieldID      = Base.FieldID;

                this.ValueString  = Base.ValueString;
                this.ValueDate    = Base.ValueDate;
                this.ValueDecimal = Base.ValueDecimal;
                this.ValueBool    = Base.ValueBool;
                this.ValueLookup  = Base.ValueLookup;
                this.ValueBinary  = Base.ValueBinary;
                this.ValueEntityReference = Base.ValueEntityReference;
                this.ValueItemReference = Base.ValueItemReference;

                this.Guid         = Base.Guid;
            }
            this.Dirty = false;

            RefreshOriginalValues();
        }


        public void FillFromDatabase(EntityBase Entity, FieldBase Field)
        {
            if (Entity.TenantID != this.TenantID ||
                Field.TenantID != this.TenantID)
            {
                Log.Fatal("TenantID Does not match. EntityID: {0}, Entity TenantID {1}, Field ID: {2}, Field TenantID : {3}, this FieldID: {4}, this TenantID {5}",
                    Entity.EntityID, Entity.TenantID, Field.FieldID, Field.TenantID, this.FieldID, this.TenantID);
                throw new FatalException("Data error.");
            }
            FieldBase Base = PersistenceService.RetreiveSingleFieldOrDefault(Entity, Field.Type);
            LoadUpValues(Base);
        }

        public void FillFromDatabase(int FieldID)
        {
            this.FieldID = FieldID;
            FieldBase Base = PersistenceService.RetreiveSingleFieldOrDefault(FieldID);

            if (Base.TenantID != this.TenantID)
            {
                Log.Fatal("TenantID Does not match. FieldID: {0}, Field TenantID: {1}, this TenantID: {2}",
                    FieldID, Base.TenantID, this.TenantID);
                throw new FatalException("Data error.");
            }

            LoadUpValues(Base);
        }

        public void FillFromView(Db.CompleteItem View)
        {

            if (this.TenantID != View.FieldTenantID)
            {
                Log.Fatal("TenantID Does not match. This TenantID: {0}, View TenantID: {1}",
                    this.TenantID, View.FieldTenantID);
                throw new FatalException("Data error.");
            }

            this.FieldID      = View.FieldID;
            this.ValueString  = View.ValueText;
            this.ValueDate    = View.ValueDate;
            this.ValueDecimal = View.ValueDecimal;
            this.ValueBool    = View.ValueBoolean;
            this.ValueLookup  = View.ValueLookup;
            this.ValueBinary  = View.ValueBinary;
            this.ValueEntityReference = View.ValueEntityReference;
            this.ValueItemReference = View.ValueItemReference;

            this.Guid         = View.EntityFieldGuid;
            this.Dirty        = false;

            RefreshOriginalValues();
        }

        /// <summary>
        /// Compares the value of this field to another and return true if they are identical. This
        /// method may be overridden in subclasses.
        /// </summary>
        /// <param name="OtherField">The other field.</param>
        /// <returns>true if the field values are identical, false otherwise</returns>
        public virtual bool IsIdenticalTo(FieldBase OtherField)
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
            if (OtherField.TenantID != this.TenantID)
            {
                Log.Error("TenantID Does not match. FieldID: {0}, Other Field ID: {1}, TenantID: {2}, Other TenantID: {3}",
                    FieldID, OtherField.FieldID, TenantID, OtherField.TenantID);
                throw new FatalException("Data error.");
            }

            if (((IField)this).Value == null && ((IField)OtherField).Value == null) // Both are null, identical
            {
                return true;
            }

            if ((((IField)this).Value == null && ((IField)OtherField).Value != null) ||  // If a is null and b is not, not identical
                (((IField)this).Value != null && ((IField)OtherField).Value == null))    // If b is null and a is not, not identical
            {
                return false;
            }

            // Both are not null, use the default comparison
            return ((IField)this).Value.Equals(((IField)OtherField).Value);
        }

        /// <summary>
        /// Persists the field to the database
        /// </summary>
        /// <param name="ctx">An optional database context. One will be created if null.</param>
        /// <returns>true if the field was updated, false if no change was made.</returns>
        public bool PersistToDatabase(Db.IDbContext ctx = null)
        {
            Log.Info("Saving the field to the database. FieldID: {0}",
                FieldID.HasValue ? FieldID.Value.ToString() : "null");

            if (!PerformSanityCheck())
            {
                Log.Fatal("TenantID mismatch. Global TenantID: {0}, Field TenantID: {1}, Entity TenantID: {2}",
                    Configure.GetTenantResolutionProvider().GetTenantID(), this.TenantID, this.Entity?.TenantID);
                throw new FatalException("Invalid data exception.");
            }

            PersistenceService.RecordAudit(this, null, null, ctx);


            ObjectIdentity Identity;
            if (this.Entity != null)
            {
                Identity = PersistenceService.SaveSingleField(this.Entity, this, ctx);
            }
            else
            {
                Identity = PersistenceService.SaveSingleField(this, ctx);
            }

            if (Identity != null)
            {
                this.FieldID = Identity.ObjectID;
                this.Guid = Identity.ObjectGuid;
            }
            else
            {
                this.FieldID = null;
                this.Guid = default(Guid);
            }

            this.Dirty = false;

            if (Identity != null && Identity.DidUpdate)
            {
                CacheService.DeleteObject(string.Format("Field:{0}", FieldID));
            }

            RefreshOriginalValues();

            return Identity == null ? false : Identity.DidUpdate;
        }

        public bool PerformSanityCheck()
        {
            int _TenantID = Configure.GetTenantResolutionProvider().GetTenantID();

            if (_TenantID != this.TenantID)
            {
                return false;
            }
            if (this.Entity != null && this.Entity.TenantID != _TenantID)
            {
                return false;
            }
            return true;
        }

        public ValidationSummary Validate()
        {
            List<ValidationResult> Errors = new List<ValidationResult>();

            if (Meta.IsRequired && (
                ((IField)this).Value == null || (this.Type.DataType == DataType.Text && string.IsNullOrWhiteSpace(((FieldString)this).Value))
            ))
            {
                Errors.Add(new ValidationResult(this, string.Format(Strings.Field_Is_Required, this.Entity.Type.Name, this.Name)));
            }

            if (Meta.HasRange && (
                (Type.DataType == DataType.Date && !TestDateRange()) ||
                (Type.DataType == DataType.Decimal && !TestDecimalRange())
            ))
            {
                Errors.Add(new ValidationResult(this, string.Format(Strings.Range_Is_Required, this.Entity.Type.Name, this.Name, this.Meta.RangeMin, this.Meta.RangeMax)));
            }

            if (Meta.HasRegex && Type.DataType == DataType.Text && !string.IsNullOrWhiteSpace(((FieldString)this).Value) && !Meta.TextRegex.IsMatch(((FieldString)this).Value))
            {
                Errors.Add(new ValidationResult(this, string.Format(Strings.Field_Does_Not_Validate, this.Entity.Type.Name, this.Name)));
            }

            return new ValidationSummary(Errors);
        }

        private bool TestDateRange()
        {
            if (Type.DataType != DataType.Date)
            {
                throw new InvalidOperationException("Cannot test a date range on anything other than a date field.");
            }

            DateTime? RangeMax = ((FieldDate)this).MetaDate.RangeMax;
            DateTime? RangeMin = ((FieldDate)this).MetaDate.RangeMin;

            if (RangeMax.HasValue && this.ValueDate.HasValue && this.ValueDate.Value > RangeMax.Value)
            {
                return false;
            }
            if (RangeMin.HasValue && this.ValueDate.HasValue && this.ValueDate.Value < RangeMin.Value)
            {
                return false;
            }

            return true;
        }

        private bool TestDecimalRange()
        {
            if (Type.DataType != DataType.Decimal)
            {
                throw new InvalidOperationException("Cannot test a date range on anything other than a date field.");
            }

            // EITODO implement this once FieldDecimal is implemented
            //decimal? RangeMax = ((FieldDecimal)this).MetaDecimal.RangeMax;
            //decimal? RangeMin = ((FieldDecimal)this).MetaDecimal.RangeMin;

            //if (RangeMax.HasValue && this.ValueDate.HasValue && this.ValueDecimal.Value > RangeMax.Value)
            //{
            //    return false;
            //}
            //if (RangeMin.HasValue && this.ValueDate.HasValue && this.ValueDecimal.Value < RangeMin.Value)
            //{
            //    return false;
            //}

            return true;
        }


        /// <summary>
        /// Sets the OriginalValue cache so long as the value is pure
        /// </summary>
        private void RefreshOriginalValues()
        {
            if (!this.Dirty)
            {
                this.OriginalValueString  = this.ValueString;
                this.OriginalValueDate    = this.ValueDate;
                this.OriginalValueDecimal = this.ValueDecimal;
                this.OriginalValueBool    = this.ValueBool;
                this.OriginalValueLookup  = this.ValueLookup;
                this.OriginalValueBinary  = this.ValueBinary;
                this.OriginalValueEntityReference = this.ValueEntityReference;
                this.OriginalValueItemReference = this.ValueItemReference;
            }
        }
    }
}
