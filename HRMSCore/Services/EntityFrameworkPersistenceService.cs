using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.Core.Models.Db;
using HRMS.Core.Models.Entities;
using HRMS.Core.Models.Fields;

namespace HRMS.Core.Services
{
    public class EntityFrameworkPersistenceService : IPersistenceService
    {
        public void SaveSingleField(IEntity Entity, IField Field)
        {
            using (var db = new HrmsDbContext())
            {
                if (!Entity.EntityID.HasValue)
                {
                    throw new ArgumentException("Must commit the entity to the database before its fields.");
                }

                Entity DbEntity = db.Entities.Where(e => e.EntityID == Entity.EntityID.Value).FirstOrDefault();

                if (DbEntity == null)
                {
                    throw new ArgumentException("The passed entity is not valid.");
                }

                EntityField DbField = null;
                if (Field.FieldID.HasValue)
                {
                    DbField = DbEntity.EntityFields.Where(f => f.EntityFieldID == Field.FieldID.Value).FirstOrDefault();
                    if (DbField == null)
                    {
                        throw new ArgumentException("The passed field ID is not valid.");
                    }

                    if (Field.Value == null)
                    {
                        DbField.IsDeleted = true;
                        db.SaveChanges();
                        return;
                    }
                }
                else
                {
                    db.Fields.Add(DbField);
                }

                DbField.ValueBoolean = null;
                DbField.ValueDate = null;
                DbField.ValueDecimal = null;
                DbField.ValueText = null;
                DbField.ValueUser = null;

                if (Field.Type.DataType == DataType.Boolean)
                {
                    DbField.ValueBoolean = (bool)Field.Value;
                }
                else if (Field.Type.DataType == DataType.Date)
                {
                    DbField.ValueDate = (DateTime)Field.Value;
                }
                else if (Field.Type.DataType == DataType.Decimal)
                {
                    DbField.ValueDecimal = (decimal)Field.Value;
                }
                else if (Field.Type.DataType == DataType.Person)
                {
                    DbField.ValueUser = (int)Field.Value;
                }
                else if (Field.Type.DataType == DataType.Text)
                {
                    DbField.ValueText = (string)Field.Value;
                }

                db.SaveChanges();
            }
        }

        public FieldT RetreiveSingleFieldOrDefault<FieldT>(IEntity Entity) where FieldT : IField, new()
        {
            if (Entity == null)
            {
                throw new ArgumentNullException();
            }
            if (!Entity.EntityID.HasValue)
            {
                throw new ArgumentException("Cannot retrieve a field without an EntityID");
            }
            FieldT Instance = new FieldT();
            int FieldTypeID = Instance.Type.DataType.Value;

            using (var db = new HrmsDbContext())
            {
                var DbField = db.Fields.Where(f => f.EntityID == Entity.EntityID.Value &&
                                              f.FieldTypeID == FieldTypeID &&
                                              !f.IsDeleted).FirstOrDefault();
                
                if (DbField == null)
                {
                    return default(FieldT);
                }

                FieldBase Base = new FieldBase(DbField.ValueText, DbField.ValueDate, DbField.ValueDecimal, DbField.ValueBoolean, DbField.ValueUser);

                FieldT Output = new FieldT();
                Output.LoadUpValues(Instance.Type, Base);

                return Output;
            }
        }
    }
}
