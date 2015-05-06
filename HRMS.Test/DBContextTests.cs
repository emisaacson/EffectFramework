using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.Core.Models;
using HRMS.Core.Models.Db;
using HRMS.Core.Services;
using Xunit;
using Ninject;
using HRMS.Core.Models.Entities;
using Microsoft.Data.Entity;
using HRMS.Core.Models.Fields;
using Microsoft.Data.Entity.Update;
using Ninject.Parameters;

namespace HRMS.Test
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class DBContextTests : IDisposable
    {
        private List<Core.Models.Db.Employee> TempEmployees = new List<Core.Models.Db.Employee>();
        private List<Entity> TempEntity = new List<Entity>();
        private List<Core.Models.Db.EmployeeRecord> TempEmployeeRecord = new List<Core.Models.Db.EmployeeRecord>();
        private List<EmployeeEntity> TempEmployeeEntity = new List<EmployeeEntity>();
        private List<EntityField> TempEntityField = new List<EntityField>();
        private bool DatabaseIsPrepared = false;

        private void PrepareEF7DatabaseIfRequired()
        {
            if (!DatabaseIsPrepared)
            {
                using (var db = new HrmsDbContext())
                using (db.Database.AsRelational().Connection.BeginTransaction())
                {
                    Core.Models.Db.Employee NewEmployee = new Core.Models.Db.Employee()
                    {
                        DisplayName = "xUnit Test Employee",
                        IsDeleted = false,
                    };

                    db.Employees.Add(NewEmployee);
                    db.SaveChanges();

                    Entity NewEntity = new Entity()
                    {
                        EffectiveDate = new DateTime(2015, 1, 1),
                        EndEffectiveDate = null,
                        EmployeeID = NewEmployee.EmployeeID.Value,
                        EntityTypeID = EntityType.Employee_General,
                        IsDeleted = false,
                    };

                    db.Entities.Add(NewEntity);
                    db.SaveChanges();

                    Core.Models.Db.EmployeeRecord NewEmployeeRecord = new Core.Models.Db.EmployeeRecord()
                    {
                        EffectiveDate = new DateTime(2015, 1, 1),
                        EndEffectiveDate = null,
                        EmployeeID = NewEmployee.EmployeeID.Value,
                        IsDeleted = false,
                    };

                    db.EmployeeRecords.Add(NewEmployeeRecord);
                    db.SaveChanges();

                    NewEmployee.EmployeeRecordID = NewEmployeeRecord.EmployeeRecordID;
                    db.SaveChanges();

                    EmployeeEntity NewEmployeeEntity = new EmployeeEntity()
                    {
                        EmployeeRecordID = NewEmployeeRecord.EmployeeRecordID.Value,
                        EntityID = NewEntity.EntityID,
                        IsDeleted = false,
                    };

                    db.EmployeeEntities.Add(NewEmployeeEntity);
                    db.SaveChanges();

                    EntityField NewField = new EntityField()
                    {
                        EntityID = NewEntity.EntityID,
                        FieldTypeID = FieldType.Hire_Date.DataType.Value,
                        IsDeleted = false,
                        ValueDate = new DateTime(2015, 1, 1),
                    };

                    db.Fields.Add(NewField);
                    db.SaveChanges();

                    db.Database.AsRelational().Connection.Transaction.Commit();

                    DatabaseIsPrepared = true;

                    TempEmployees.Add(NewEmployee);
                    TempEntity.Add(NewEntity);
                    TempEmployeeRecord.Add(NewEmployeeRecord);
                    TempEmployeeEntity.Add(NewEmployeeEntity);
                    TempEntityField.Add(NewField);
                }
            }
        }

        private void TearDownEF7DatabaseIfRequired()
        {
            if (DatabaseIsPrepared)
            {
                using (var db = new HrmsDbContext())
                using (db.Database.AsRelational().Connection.BeginTransaction())
                {
                    foreach (var Field in TempEntityField)
                    {
                        db.Fields.Remove(Field);
                    }
                    db.SaveChanges();

                    foreach (var Employee in TempEmployees)
                    {
                        Employee.EmployeeRecordID = null;
                    }
                    db.SaveChanges();

                    var Command = db.Database.AsSqlServer().Connection.DbConnection.CreateCommand();
                    Command.CommandText = @"
                            ALTER TABLE EmployeeRecords NOCHECK CONSTRAINT all;
                            ALTER TABLE EmployeeEntities NOCHECK CONSTRAINT all;
                            ALTER TABLE Employees NOCHECK CONSTRAINT all;";
                    Command.Transaction = db.Database.AsRelational().Connection.DbTransaction;
                    Command.ExecuteNonQuery();

                    foreach (var EmployeeRecord in TempEmployeeRecord)
                    {
                        db.EmployeeRecords.Remove(EmployeeRecord);
                    }
                    db.SaveChanges();

                    foreach (var EmployeeEntity in TempEmployeeEntity)
                    {
                        db.EmployeeEntities.Remove(EmployeeEntity);
                    }
                    db.SaveChanges();

                    foreach (var Entity in TempEntity)
                    {
                        db.Entities.Remove(Entity);
                    }
                    db.SaveChanges();

                    foreach (var Employee in TempEmployees)
                    {
                        db.Employees.Remove(Employee);
                    }
                    db.SaveChanges();

                    var Command2 = db.Database.AsSqlServer().Connection.DbConnection.CreateCommand();
                    Command2.CommandText = @"
                            ALTER TABLE EmployeeRecords WITH CHECK CHECK CONSTRAINT all;
                            ALTER TABLE EmployeeEntities WITH CHECK CHECK CONSTRAINT all;
                            ALTER TABLE Employees WITH CHECK CHECK CONSTRAINT all;";
                    Command2.Transaction = db.Database.AsRelational().Connection.DbTransaction;
                    Command2.ExecuteNonQuery();

                    db.Database.AsRelational().Connection.Transaction.Commit();

                    TempEntityField.Clear();
                    TempEmployees.Clear();
                    TempEmployeeRecord.Clear();
                    TempEmployeeEntity.Clear();
                    TempEntity.Clear();
                }
            }
        }

        public DBContextTests()
        {
            PrepareEF7DatabaseIfRequired();
        }

        [Fact]
        public void CreateEF7DBContext()
        {
            using (var db = new HrmsDbContext())
            {

            }
        }

        [Fact]
        public void LoadUpEmployeeObject()
        {
            using (IKernel Kernel = new StandardKernel())
            {
                Kernel.Load(new HRMS.Core.Configure());

                Core.Models.Db.Employee Employee = TempEmployees.First();

                Core.Models.Employee NewEmployee = Kernel.Get<Core.Models.Employee>(new ConstructorArgument("EmployeeID", Employee.EmployeeID.Value));
                NewEmployee.Load();

                Assert.Equal(Employee.EmployeeID, NewEmployee.EmployeeID);
                Assert.Equal(1, NewEmployee.EmployeeRecords.Count());
                Assert.Equal(1, NewEmployee.EmployeeRecords.First().Value.AllEntities.Count());

                EmployeeGeneralEntity GeneralEntity = NewEmployee.EmployeeRecords.First().Value.GetFirstEntityOrDefault<EmployeeGeneralEntity>();

                Assert.NotNull(GeneralEntity);

                Assert.Equal(TempEntityField.First().ValueDate.Value, GeneralEntity.HireDate.Value);

            }
        }

        public void Dispose()
        {
            TearDownEF7DatabaseIfRequired();
        }
    }
}
