using System;
using EffectFramework.Core.Models.Entities;
using EffectFramework.Core.Services;

namespace EffectFramework.Core.Models.Fields
{
    public class FieldBase
    {
        public FieldType Type { get; protected set; }
        public Guid Guid { get; protected set; }
        public int? FieldID { get; protected set; }
        public bool Dirty { get; protected set; }

        protected string ValueString { get; set; }
        protected DateTime? ValueDate { get; set; }
        protected decimal? ValueDecimal { get; set; }
        protected bool? ValueBool { get; set; }
        protected int? ValueLookup { get; set; }
        protected byte[] ValueBinary { get; set; }
        protected readonly IPersistenceService PersistenceService;

        protected void LoadUpValues(FieldBase Base)
        {
            this.Dirty = false;
            if (Base == null)
            {
                this.FieldID      = null;
                this.ValueString  = null;
                this.ValueDate    = null;
                this.ValueDecimal = null;
                this.ValueBool    = null;
                this.ValueLookup  = null;
                this.ValueBinary  = null;
            }
            else
            {
                this.FieldID      = Base.FieldID;
                this.ValueString  = Base.ValueString;
                this.ValueDate    = Base.ValueDate;
                this.ValueDecimal = Base.ValueDecimal;
                this.ValueBool    = Base.ValueBool;
                this.ValueLookup  = Base.ValueLookup;
                this.ValueBinary  = Base.ValueBinary;
                this.Guid         = Base.Guid;
            }
        }

        public FieldBase(IPersistenceService PersistenceService)
        {
            this.Dirty = false;
            this.PersistenceService = PersistenceService;
        }

        public FieldBase(Db.Field Field)
        {
            this.Dirty        = false;
            this.FieldID      = Field.FieldID;
            this.ValueString  = Field.ValueText;
            this.ValueDate    = Field.ValueDate;
            this.ValueDecimal = Field.ValueDecimal;
            this.ValueBool    = Field.ValueBoolean;
            this.ValueLookup  = Field.ValueLookup;
            this.ValueBinary  = Field.ValueBinary;
            this.Guid         = Field.Guid;
        }

        public FieldBase(FieldType Type, FieldBase Base, IPersistenceService PersistenceService)
        {
            this.PersistenceService = PersistenceService;
            this.Type = Type;
            LoadUpValues(Base);
        }

        public void FillFromDatabase(EntityBase Entity, FieldBase Field)
        {
            FieldBase Base = PersistenceService.RetreiveSingleFieldOrDefault(Entity, Field.Type);
            LoadUpValues(Base);
        }

        public void FillFromDatabase(int FieldID)
        {
            this.FieldID = FieldID;
            FieldBase Base = PersistenceService.RetreiveSingleFieldOrDefault(FieldID);
            LoadUpValues(Base);
        }

        public void PersistToDatabase(Db.IDbContext ctx = null)
        {
            var Identity = PersistenceService.SaveSingleField(this, ctx);
            this.FieldID = Identity.ObjectID;
            this.Guid = Identity.ObjectGuid;
            this.Dirty = false;
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
                throw new ArgumentNullException();
            }
            if (OtherField.Type.DataType != this.Type.DataType)
            {
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

            // Both are not null, use the default comparison
            return ((IField)this).Value.Equals(((IField)OtherField).Value);
        }

        public void PersistToDatabase(EntityBase Entity, Db.IDbContext ctx = null)
        {
            if (Entity == null)
            {
                throw new ArgumentNullException();
            }

            var Identity = PersistenceService.SaveSingleField(Entity, this, ctx);
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
        }
    }
}
