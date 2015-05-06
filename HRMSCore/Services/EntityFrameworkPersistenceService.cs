using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.Core.Models;
using HRMS.Core.Models.Db;
using HRMS.Core.Models.Entities;
using HRMS.Core.Models.Fields;
using Microsoft.Data.Entity;

namespace HRMS.Core.Services
{
    public class EntityFrameworkPersistenceService : IPersistenceService
    {
        public void SaveSingleField(EntityBase Entity, IField Field)
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
                    DbField = null;// DbEntity.EntityFields.Where(f => f.EntityFieldID == Field.FieldID.Value).FirstOrDefault();
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

        public FieldBase RetreiveSingleFieldOrDefault(EntityBase Entity, FieldType FieldType)
        {

            int FieldTypeID = FieldType.DataType.Value;

            return RetreiveSingleFieldOrDefault(Entity, FieldTypeID);
        }

        public FieldBase RetreiveSingleFieldOrDefault(EntityBase Entity, int FieldTypeID)
        {
            if (Entity == null)
            {
                throw new ArgumentNullException();
            }
            if (!Entity.EntityID.HasValue)
            {
                throw new ArgumentException("Cannot retrieve a field without an EntityID");
            }

            using (var db = new HrmsDbContext())
            {
                var DbField = db.Fields.Where(f => f.EntityID == Entity.EntityID.Value &&
                                              f.FieldTypeID == FieldTypeID &&
                                              !f.IsDeleted).FirstOrDefault();

                if (DbField == null)
                {
                    return null;
                }

                FieldBase Base = new FieldBase(DbField.ValueText, DbField.ValueDate, DbField.ValueDecimal, DbField.ValueBoolean, DbField.ValueUser);

                return Base;
            }
        }

        public FieldBase RetreiveSingleFieldOrDefault<FieldT>(EntityBase Entity) where FieldT : IField, new()
        {

            FieldT Instance = new FieldT();
            int FieldTypeID = Instance.Type.DataType.Value;

            return RetreiveSingleFieldOrDefault(Entity, FieldTypeID);
        }

        public EntityT RetreiveSingleEntityOrDefault<EntityT>(Models.EmployeeRecord EmployeeRecord) where EntityT : EntityBase, new()
        {
            if (EmployeeRecord == null)
            {
                throw new ArgumentNullException();
            }
            if (!EmployeeRecord.EmployeeRecordID.HasValue)
            {
                throw new ArgumentException("Cannot fetch an entity from an employee record with a null ID.");
            }

            return RetreiveSingleEntityOrDefault<EntityT>(EmployeeRecord.EmployeeRecordID.Value);
        }

        public EntityT RetreiveSingleEntityOrDefault<EntityT>(int EmployeeRecordID) where EntityT : EntityBase, new()
        {
            if (EmployeeRecordID < 1)
            {
                throw new ArgumentOutOfRangeException();
            }

            EntityT Instance = new EntityT();
            Instance.PersistenceService = this;

            using (var db = new HrmsDbContext())
            using (db.Database.AsRelational().Connection.BeginTransaction())
            {
                var EntityIDs = db.EmployeeEntities
                    .Where(er =>
                        er.EmployeeRecordID == EmployeeRecordID &&
                        !er.IsDeleted)
                    .Select(er =>
                        er.EntityID
                    );

                var DbEntity = db.Entities
                    .Where(e =>
                        EntityIDs.Contains(e.EntityID) &&
                        e.EntityTypeID == Instance.Type.Value &&
                        !e.IsDeleted)
                    .FirstOrDefault();

                if (DbEntity != null)
                {
                    EntityT Output = EntityFactory.GenerateEntityFromDbObject<EntityT>(DbEntity);
                }

                return default(EntityT);
            }
        }

        public List<EntityBase> RetreiveAllEntities(Models.EmployeeRecord EmployeeRecord)
        {
            if (EmployeeRecord == null)
            {
                throw new ArgumentNullException();
            }
            if (!EmployeeRecord.EmployeeRecordID.HasValue)
            {
                throw new ArgumentException("Record must have an ID.");
            }

            return RetreiveAllEntities(EmployeeRecord.EmployeeRecordID.Value);
        }

        public List<EntityBase> RetreiveAllEntities(int EmployeeRecordID)
        {
            using (var db = new HrmsDbContext())
            {
                var EntityIDs = db.EmployeeEntities
                    .Where(er =>
                        er.EmployeeRecordID == EmployeeRecordID &&
                        !er.IsDeleted)
                    .Select(er =>
                        er.EntityID
                    );

                var DbEntities = db.Entities
                    .Where(e =>
                        EntityIDs.Contains(e.EntityID) &&
                        !e.IsDeleted)
                    .ToList();

                List<EntityBase> Output = new List<EntityBase>();
                foreach (var DbEntity in DbEntities)
                {
                    Output.Add(EntityFactory.GenerateEntityFromDbObject(DbEntity));
                }

                return Output;
            }
        }

        public List<Models.EmployeeRecord> RetreiveAllEmployeeRecords(Models.Employee Employee)
        {
            if (Employee == null)
            {
                throw new ArgumentNullException();
            }
            if (!Employee.EmployeeID.HasValue)
            {
                throw new ArgumentException("Employee must has a valid ID.");
            }

            return RetreiveAllEmployeeRecords(Employee.EmployeeID.Value);
        }

        public List<Models.EmployeeRecord> RetreiveAllEmployeeRecords(int EmployeeID)
        {
            using (var db = new HrmsDbContext())
            {
                var DbEmployeeRecords = db.EmployeeRecords
                    .Where(er =>
                        er.EmployeeID == EmployeeID &&
                        er.IsDeleted == false)
                    .ToList();

                List<Models.EmployeeRecord> Output = new List<Models.EmployeeRecord>();

                foreach (var DbEmployeeRecord in DbEmployeeRecords)
                {
                    var EmployeeRecord = new Models.EmployeeRecord(DbEmployeeRecord, this);
                    Output.Add(EmployeeRecord);
                }

                return Output;
            }
        }

        public Models.Db.EmployeeRecord RetreiveSingleDbEmployeeRecord(int EmployeeRecordID)
        {
            using (var db = new HrmsDbContext())
            {
                var DbEmployeeRecord = db.EmployeeRecords
                    .Where(er =>
                        er.EmployeeRecordID == EmployeeRecordID &&
                        er.IsDeleted == false)
                    .FirstOrDefault();

                return DbEmployeeRecord;
            }
        }
    }
}
