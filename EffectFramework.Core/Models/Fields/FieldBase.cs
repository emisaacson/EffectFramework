using System;
using EffectFramework.Core.Models.Entities;
using EffectFramework.Core.Services;

namespace EffectFramework.Core.Models.Fields
{
    /// <summary>
    /// The base class of all Fields
    /// </summary>
    public class FieldBase
    {
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
        /// The static FielType of this Field
        /// </value>
        public FieldType Type { get; protected set; }
        public EntityBase Entity { get; protected set; }
        public Guid Guid { get; protected set; }
        public int? FieldID { get; protected set; }
        public bool Dirty { get; protected set; }

        protected string ValueString { get; set; }
        protected DateTime? ValueDate { get; set; }
        protected decimal? ValueDecimal { get; set; }
        protected bool? ValueBool { get; set; }
        protected int? ValueLookup { get; set; }
        protected byte[] ValueBinary { get; set; }

        protected string OriginalValueString { get; set; }
        protected DateTime? OriginalValueDate { get; set; }
        protected decimal? OriginalValueDecimal { get; set; }
        protected bool? OriginalValueBool { get; set; }
        protected int? OriginalValueLookup { get; set; }
        protected byte[] OriginalValueBinary { get; set; }

        protected readonly IPersistenceService PersistenceService;

        public FieldBase(IPersistenceService PersistenceService)
        {
            this.Dirty = false;
            this.PersistenceService = PersistenceService;
        }

        public FieldBase(Db.Field Field)
        {
            this.Dirty = false;
            this.FieldID = Field.FieldID;

            this.ValueString  = Field.ValueText;
            this.ValueDate    = Field.ValueDate;
            this.ValueDecimal = Field.ValueDecimal;
            this.ValueBool    = Field.ValueBoolean;
            this.ValueLookup  = Field.ValueLookup;
            this.ValueBinary  = Field.ValueBinary;
            this.Guid = Field.Guid;

            RefreshOriginalValues();

        }

        public FieldBase(FieldType Type, FieldBase Base, IPersistenceService PersistenceService)
        {
            this.PersistenceService = PersistenceService;
            LoadUpValues(Base);
        }

        protected void LoadUpValues(FieldBase Base)
        {
            Log.Trace("Loading values for Field. FieldID: {0}",
                Base == null || !Base.FieldID.HasValue ? "null" : Base.FieldID.Value.ToString());

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

            RefreshOriginalValues();
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

        public void FillFromView(Db.CompleteItem View)
        {
            this.Dirty        = false;

            this.FieldID      = View.FieldID;
            this.ValueString  = View.ValueText;
            this.ValueDate    = View.ValueDate;
            this.ValueDecimal = View.ValueDecimal;
            this.ValueBool    = View.ValueBoolean;
            this.ValueLookup  = View.ValueLookup;
            this.ValueBinary  = View.ValueBinary;

            this.Guid         = View.EntityFieldGuid;

            RefreshOriginalValues();
        }

        public void PersistToDatabase(Db.IDbContext ctx = null)
        {
            Log.Info("Saving the field to the database. FieldID: {0}",
                FieldID.HasValue ? FieldID.Value.ToString() : "null");

            var Identity = PersistenceService.SaveSingleField(this, ctx);
            this.FieldID = Identity.ObjectID;
            this.Guid = Identity.ObjectGuid;
            this.Dirty = false;

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

            // Both are not null, use the default comparison
            return ((IField)this).Value.Equals(((IField)OtherField).Value);
        }

        public void PersistToDatabase(EntityBase Entity, Db.IDbContext ctx = null)
        {
            if (Entity == null)
            {
                Log.Warn("Trying to save a Field to a null Entity. FieldID: {0}, Field Type: {1}",
                    FieldID.HasValue ? FieldID.Value.ToString() : "null",
                    Type.Name);

                throw new ArgumentNullException();
            }

            Log.Info("Saving the field to the database. FieldID: {0}, EntityID: {1}",
                FieldID.HasValue ? FieldID.Value.ToString() : "null",
                Entity.EntityID.HasValue ? Entity.EntityID.Value.ToString() : "null");

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

            RefreshOriginalValues();
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
            }
        }
    }
}
