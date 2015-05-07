using System;
using System.Collections.Generic;
using System.Linq;
using HRMS.Core.Models;
using HRMS.Core.Models.Db;
using HRMS.Core.Models.Entities;
using HRMS.Core.Models.Fields;
using Microsoft.Data.Entity;

namespace HRMS.Core.Services
{
    public class EntityFrameworkPersistenceService : IPersistenceService
    {
        public Guid SaveSingleField(EntityBase Entity, FieldBase Field, IDbContext ctx = null)
        {
            HrmsDb7Context db = null;
            try
            {
                if (ctx == null)
                {
                    db = new HrmsDb7Context();
                }
                else
                {
                    db = (HrmsDb7Context)ctx;
                }

                EntityField DbField = null;
                bool CreatedAnew = false;
                if (!Field.FieldID.HasValue)
                {
                    DbField = new EntityField()
                    {
                        IsDeleted = false,
                        Guid = Guid.NewGuid(),
                    };
                    db.Fields.Add(DbField);
                    db.SaveChanges();
                    CreatedAnew = true;
                }
                else {
                    DbField = db.Fields.Where(ef => ef.EntityFieldID == Field.FieldID.Value && ef.IsDeleted == false).FirstOrDefault();
                }

                if (DbField == null)
                {
                    throw new ArgumentException("The passed field ID is not valid.");
                }

                if (!CreatedAnew && DbField.Guid != Field.Guid)
                {
                    throw new Exceptions.GuidMismatchException();
                }

                if (!Field.Dirty)
                {
                    return Field.Guid;
                }

                if (((IField)Field).Value == null)
                {
                    DbField.IsDeleted = true;
                    db.SaveChanges();
                    return Field.Guid;
                }

                DbField.ValueBoolean = null;
                DbField.ValueDate = null;
                DbField.ValueDecimal = null;
                DbField.ValueText = null;
                DbField.ValueUser = null;
                DbField.Guid = Guid.NewGuid();

                if (Field.Type.DataType == DataType.Boolean)
                {
                    DbField.ValueBoolean = (bool)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Date)
                {
                    DbField.ValueDate = (DateTime)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Decimal)
                {
                    DbField.ValueDecimal = (decimal)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Person)
                {
                    DbField.ValueUser = (int)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Text)
                {
                    DbField.ValueText = (string)((IField)Field).Value;
                }

                db.SaveChanges();

                return DbField.Guid;
            }
            finally
            {
                if (db != null && ctx == null)
                {
                    db.Dispose();
                }
            }
        }
        public Guid SaveSingleField(FieldBase Field, IDbContext ctx = null)
        {
            HrmsDb7Context db = null;
            try {
                if (ctx == null)
                {
                    db = new HrmsDb7Context();
                }
                else
                {
                    db = (HrmsDb7Context)ctx;
                }

                if (!Field.FieldID.HasValue)
                {
                    throw new InvalidOperationException("Must create a new field in the context of an entity.");
                }

                EntityField DbField = db.Fields.Where(ef => ef.EntityFieldID == Field.FieldID.Value && ef.IsDeleted == false).FirstOrDefault();
                if (DbField == null)
                {
                    throw new ArgumentException("The passed field ID is not valid.");
                }

                if (DbField.Guid != Field.Guid)
                {
                    throw new Exceptions.GuidMismatchException();
                }

                if (!Field.Dirty)
                {
                    return Field.Guid;
                }

                if (((IField)Field).Value == null)
                {
                    DbField.IsDeleted = true;
                    db.SaveChanges();
                    return Field.Guid;
                }

                DbField.ValueBoolean = null;
                DbField.ValueDate = null;
                DbField.ValueDecimal = null;
                DbField.ValueText = null;
                DbField.ValueUser = null;
                DbField.Guid = Guid.NewGuid();

                if (Field.Type.DataType == DataType.Boolean)
                {
                    DbField.ValueBoolean = (bool)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Date)
                {
                    DbField.ValueDate = (DateTime)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Decimal)
                {
                    DbField.ValueDecimal = (decimal)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Person)
                {
                    DbField.ValueUser = (int)((IField)Field).Value;
                }
                else if (Field.Type.DataType == DataType.Text)
                {
                    DbField.ValueText = (string)((IField)Field).Value;
                }

                db.SaveChanges();

                return DbField.Guid;
            }
            finally
            {
                if (db != null && ctx == null)
                {
                    db.Dispose();
                }
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

            using (var db = new HrmsDb7Context())
            {
                var DbField = db.Fields.Where(f => f.EntityID == Entity.EntityID.Value &&
                                              f.FieldTypeID == FieldTypeID &&
                                              !f.IsDeleted).FirstOrDefault();

                if (DbField == null)
                {
                    return null;
                }

                FieldBase Base = new FieldBase(DbField);

                return Base;
            }
        }

        public FieldBase RetreiveSingleFieldOrDefault(int FieldID)
        {

            using (var db = new HrmsDb7Context())
            {
                var DbField = db.Fields.Where(f => f.EntityFieldID == FieldID &&
                                              !f.IsDeleted).FirstOrDefault();

                if (DbField == null)
                {
                    return null;
                }

                FieldBase Base = new FieldBase(DbField);

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

            using (var db = new HrmsDb7Context())
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
            using (var db = new HrmsDb7Context())
            {
                var EntityIDs = db.EmployeeEntities
                    .Where(er =>
                        er.EmployeeRecordID == EmployeeRecordID &&
                        !er.IsDeleted)
                    .Select(er =>
                        er.EntityID
                    ).ToArray();

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
            using (var db = new HrmsDb7Context())
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
            using (var db = new HrmsDb7Context())
            {
                var DbEmployeeRecord = db.EmployeeRecords
                    .Where(er =>
                        er.EmployeeRecordID == EmployeeRecordID &&
                        er.IsDeleted == false)
                    .FirstOrDefault();

                return DbEmployeeRecord;
            }
        }

        public Guid RetreiveGuidForEmployeeRecord(int EmployeeID)
        {
            using (var db = new HrmsDb7Context())
            {
                var DbEmployeeRecord = db.Employees
                    .Where(e =>
                        e.EmployeeID == EmployeeID &&
                        e.IsDeleted == false)
                    .FirstOrDefault();

                if (DbEmployeeRecord != null)
                {
                    return DbEmployeeRecord.Guid;
                }

                throw new ArgumentException("Invalid Employee ID passed.");
            }
        }
    }
}
