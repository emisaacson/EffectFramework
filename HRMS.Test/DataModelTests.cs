using System;
using System.Collections.Generic;
using System.Linq;
using HRMS.Core.Models;
using HRMS.Core.Models.Db;
using Xunit;
using Ninject;
using HRMS.Core.Models.Entities;
using Microsoft.Data.Entity;
using HRMS.Core.Models.Fields;
using Ninject.Parameters;
using HRMS.Core;

namespace HRMS.Test
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class DataModelTests : IDisposable
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
                using (var db = new HrmsDb7Context())
                using (db.Database.AsRelational().Connection.BeginTransaction())
                {
                    Core.Models.Db.Employee NewEmployee = new Core.Models.Db.Employee()
                    {
                        DisplayName = "xUnit Test Employee",
                        IsDeleted = false,
                        Guid = Guid.NewGuid(),
                    };

                    db.Employees.Add(NewEmployee);
                    db.SaveChanges();

                    Entity NewEntity1 = new Entity()
                    {
                        EffectiveDate = new DateTime(2015, 1, 1),
                        EndEffectiveDate = null,
                        EmployeeID = NewEmployee.EmployeeID.Value,
                        EntityTypeID = EntityType.Employee_General,
                        IsDeleted = false,
                        Guid = Guid.NewGuid(),
                    };


                    Entity NewEntity2 = new Entity()
                    {
                        EffectiveDate = new DateTime(2015, 1, 1),
                        EndEffectiveDate = new DateTime(2015, 2, 1),
                        EmployeeID = NewEmployee.EmployeeID.Value,
                        EntityTypeID = EntityType.Job,
                        IsDeleted = false,
                        Guid = Guid.NewGuid(),
                    };

                    Entity NewEntity3 = new Entity()
                    {
                        EffectiveDate = new DateTime(2015, 2, 1),
                        EndEffectiveDate = null,
                        EmployeeID = NewEmployee.EmployeeID.Value,
                        EntityTypeID = EntityType.Job,
                        IsDeleted = false,
                        Guid = Guid.NewGuid(),
                    };

                    db.Entities.Add(NewEntity1);
                    db.Entities.Add(NewEntity2);
                    db.Entities.Add(NewEntity3);
                    db.SaveChanges();

                    Core.Models.Db.EmployeeRecord NewEmployeeRecord1 = new Core.Models.Db.EmployeeRecord()
                    {
                        EffectiveDate = new DateTime(2015, 1, 1),
                        EndEffectiveDate = new DateTime(2015, 2, 1),
                        EmployeeID = NewEmployee.EmployeeID.Value,
                        IsDeleted = false,
                        Guid = Guid.NewGuid(),
                    };

                    Core.Models.Db.EmployeeRecord NewEmployeeRecord2 = new Core.Models.Db.EmployeeRecord()
                    {
                        EffectiveDate = new DateTime(2015, 2, 1),
                        EndEffectiveDate = null,
                        EmployeeID = NewEmployee.EmployeeID.Value,
                        IsDeleted = false,
                        Guid = Guid.NewGuid(),
                    };

                    db.EmployeeRecords.Add(NewEmployeeRecord1);
                    db.EmployeeRecords.Add(NewEmployeeRecord2);
                    db.SaveChanges();

                    NewEmployee.EmployeeRecordID = NewEmployeeRecord2.EmployeeRecordID;
                    db.SaveChanges();

                    EmployeeEntity NewEmployeeEntity1 = new EmployeeEntity()
                    {
                        EmployeeRecordID = NewEmployeeRecord1.EmployeeRecordID.Value,
                        EntityID = NewEntity1.EntityID,
                        IsDeleted = false,
                        Guid = Guid.NewGuid(),
                    };

                    EmployeeEntity NewEmployeeEntity2 = new EmployeeEntity()
                    {
                        EmployeeRecordID = NewEmployeeRecord1.EmployeeRecordID.Value,
                        EntityID = NewEntity2.EntityID,
                        IsDeleted = false,
                        Guid = Guid.NewGuid(),
                    };

                    EmployeeEntity NewEmployeeEntity3 = new EmployeeEntity()
                    {
                        EmployeeRecordID = NewEmployeeRecord2.EmployeeRecordID.Value,
                        EntityID = NewEntity1.EntityID,
                        IsDeleted = false,
                        Guid = Guid.NewGuid(),
                    };

                    EmployeeEntity NewEmployeeEntity4 = new EmployeeEntity()
                    {
                        EmployeeRecordID = NewEmployeeRecord2.EmployeeRecordID.Value,
                        EntityID = NewEntity3.EntityID,
                        IsDeleted = false,
                        Guid = Guid.NewGuid(),
                    };

                    db.EmployeeEntities.Add(NewEmployeeEntity1);
                    db.EmployeeEntities.Add(NewEmployeeEntity2);
                    db.EmployeeEntities.Add(NewEmployeeEntity3);
                    db.EmployeeEntities.Add(NewEmployeeEntity4);
                    db.SaveChanges();

                    EntityField NewField1 = new EntityField()
                    {
                        EntityID = NewEntity1.EntityID,
                        FieldTypeID = FieldType.Hire_Date.DataType.Value,
                        IsDeleted = false,
                        ValueDate = new DateTime(2015, 1, 1),
                        Guid = Guid.NewGuid(),
                    };

                    EntityField NewField2 = new EntityField()
                    {
                        EntityID = NewEntity2.EntityID,
                        FieldTypeID = FieldType.Job_Title.DataType.Value,
                        IsDeleted = false,
                        ValueText = "Manager",
                        Guid = Guid.NewGuid(),
                    };

                    EntityField NewField3 = new EntityField()
                    {
                        EntityID = NewEntity3.EntityID,
                        FieldTypeID = FieldType.Job_Title.DataType.Value,
                        IsDeleted = false,
                        ValueText = "Director",
                        Guid = Guid.NewGuid(),
                    };

                    db.Fields.Add(NewField1);
                    db.Fields.Add(NewField2);
                    db.Fields.Add(NewField3);
                    db.SaveChanges();

                    db.Database.AsRelational().Connection.Transaction.Commit();

                    DatabaseIsPrepared = true;

                    TempEmployees.Add(NewEmployee);
                    TempEntity.Add(NewEntity1);
                    TempEntity.Add(NewEntity2);
                    TempEntity.Add(NewEntity3);
                    TempEmployeeRecord.Add(NewEmployeeRecord1);
                    TempEmployeeRecord.Add(NewEmployeeRecord2);
                    TempEmployeeEntity.Add(NewEmployeeEntity1);
                    TempEmployeeEntity.Add(NewEmployeeEntity2);
                    TempEmployeeEntity.Add(NewEmployeeEntity3);
                    TempEmployeeEntity.Add(NewEmployeeEntity4);
                    TempEntityField.Add(NewField1);
                    TempEntityField.Add(NewField2);
                    TempEntityField.Add(NewField3);
                }
            }
        }

        private void TearDownEF7DatabaseIfRequired()
        {
            if (DatabaseIsPrepared)
            {
                using (var db = new HrmsDb7Context())
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

        public DataModelTests()
        {
            PrepareEF7DatabaseIfRequired();
        }

        [Fact]
        public void CreateEF7DBContext()
        {
            using (var db = new HrmsDb7Context())
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
                NewEmployee.ChangeEffectiveDate(new DateTime(2015, 1, 1));

                Assert.Equal(Employee.EmployeeID, NewEmployee.EmployeeID);
                Assert.Equal(2, NewEmployee.EmployeeRecords.Count());
                Assert.Equal(2, NewEmployee.EmployeeRecords.First().Value.AllEntities.Count());
                Assert.Equal(2, NewEmployee.EmployeeRecords.Last().Value.AllEntities.Count());
                Assert.False(NewEmployee.EmployeeRecords.First().Value.Dirty);
                Assert.False(NewEmployee.EmployeeRecords.Last().Value.Dirty);

            }
        }

        [Fact]
        public void RetreiveEntitiesByEffectiveDate()
        {
            using (IKernel Kernel = new StandardKernel())
            {
                Kernel.Load(new HRMS.Core.Configure());
                Core.Models.Db.Employee Employee = TempEmployees.First();

                Core.Models.Employee NewEmployee = Kernel.Get<Core.Models.Employee>(new ConstructorArgument("EmployeeID", Employee.EmployeeID.Value));
                NewEmployee.Load();
                NewEmployee.ChangeEffectiveDate(new DateTime(2015, 1, 1));

                Assert.False(NewEmployee.Dirty);
                Assert.Equal(Employee.Guid, NewEmployee.Guid);

                EmployeeGeneralEntity GeneralEntity = NewEmployee.EmployeeRecords.First().Value.GetFirstEntityOrDefault<EmployeeGeneralEntity>();

                Assert.NotNull(GeneralEntity);
                Assert.Equal(TempEntityField.First().ValueDate.Value, GeneralEntity.HireDate.Value);

                JobEntity FirstJob = NewEmployee.EffectiveRecord.GetFirstEntityOrDefault<JobEntity>();

                Assert.NotNull(FirstJob);
                Assert.Equal("Manager", FirstJob.JobTitle.Value);

                NewEmployee.ChangeEffectiveDate(new DateTime(2015, 2, 2));

                JobEntity SecondJob = NewEmployee.EffectiveRecord.GetFirstEntityOrDefault<JobEntity>();

                Assert.NotNull(SecondJob);
                Assert.Equal("Director", SecondJob.JobTitle.Value);
            }
        }

        [Fact]
        public void MakeSureEntityFieldsArePopulated()
        {
            using (IKernel Kernel = new StandardKernel())
            {
                Kernel.Load(new HRMS.Core.Configure());
                Core.Models.Db.Employee Employee = TempEmployees.First();

                Core.Models.Employee NewEmployee = Kernel.Get<Core.Models.Employee>(new ConstructorArgument("EmployeeID", Employee.EmployeeID.Value));
                NewEmployee.Load();
                NewEmployee.ChangeEffectiveDate(new DateTime(2015, 1, 1));

                EmployeeGeneralEntity GeneralEntity = NewEmployee.EmployeeRecords.First().Value.GetFirstEntityOrDefault<EmployeeGeneralEntity>();

                Assert.NotNull(GeneralEntity);
                Assert.NotNull(GeneralEntity.EntityID);
                Assert.NotEqual(Guid.Empty, GeneralEntity.Guid);
                Assert.False(GeneralEntity.Dirty);
                Assert.Equal(Strings.Employee_General, GeneralEntity.Type.Name);

                JobEntity FirstJob = NewEmployee.EffectiveRecord.GetFirstEntityOrDefault<JobEntity>();

                Assert.NotNull(FirstJob);
                Assert.NotNull(FirstJob.EntityID);
                Assert.NotEqual(Guid.Empty, FirstJob.Guid);
                Assert.False(FirstJob.Dirty);
                Assert.Equal(Strings.Job, FirstJob.Type.Name);

                NewEmployee.ChangeEffectiveDate(new DateTime(2015, 2, 2));

                JobEntity SecondJob = NewEmployee.EffectiveRecord.GetFirstEntityOrDefault<JobEntity>();

                Assert.NotNull(SecondJob);
                Assert.NotNull(SecondJob.EntityID);
                Assert.NotEqual(Guid.Empty, SecondJob.Guid);
                Assert.False(SecondJob.Dirty);
                Assert.Equal(Strings.Job, SecondJob.Type.Name);
            }
        }

        [Fact]
        public void MakeSureFieldFieldsArePopulated()
        {
            using (IKernel Kernel = new StandardKernel())
            {
                Kernel.Load(new HRMS.Core.Configure());
                Core.Models.Db.Employee Employee = TempEmployees.First();

                Core.Models.Employee NewEmployee = Kernel.Get<Core.Models.Employee>(new ConstructorArgument("EmployeeID", Employee.EmployeeID.Value));
                NewEmployee.Load();
                NewEmployee.ChangeEffectiveDate(new DateTime(2015, 1, 1));

                EmployeeGeneralEntity GeneralEntity = NewEmployee.EmployeeRecords.First().Value.GetFirstEntityOrDefault<EmployeeGeneralEntity>();

                Assert.NotNull(GeneralEntity);
                Assert.NotNull(GeneralEntity.HireDate);
                Assert.NotNull(GeneralEntity.HireDate.FieldID);
                Assert.NotEqual(Guid.Empty, GeneralEntity.HireDate.Guid);
                Assert.False(GeneralEntity.HireDate.Dirty);
                Assert.Equal(Strings.Hire_Date, GeneralEntity.HireDate.Type.Name);
                Assert.Equal(Strings.Hire_Date, GeneralEntity.HireDate.Name);

                JobEntity FirstJob = NewEmployee.EffectiveRecord.GetFirstEntityOrDefault<JobEntity>();

                Assert.NotNull(FirstJob);
                Assert.NotNull(FirstJob.JobTitle);
                Assert.NotNull(FirstJob.JobTitle.FieldID);
                Assert.NotEqual(Guid.Empty, FirstJob.JobTitle.Guid);
                Assert.False(FirstJob.JobTitle.Dirty);
                Assert.Equal(Strings.Job_Title, FirstJob.JobTitle.Type.Name);
                Assert.Equal(Strings.Job_Title, FirstJob.JobTitle.Name);

                NewEmployee.ChangeEffectiveDate(new DateTime(2015, 2, 2));

                JobEntity SecondJob = NewEmployee.EffectiveRecord.GetFirstEntityOrDefault<JobEntity>();

                Assert.NotNull(SecondJob);
                Assert.NotNull(SecondJob.JobTitle);
                Assert.NotNull(SecondJob.JobTitle.FieldID);
                Assert.NotEqual(Guid.Empty, SecondJob.JobTitle.Guid);
                Assert.False(SecondJob.JobTitle.Dirty);
                Assert.Equal(Strings.Job_Title, SecondJob.JobTitle.Type.Name);
                Assert.Equal(Strings.Job_Title, SecondJob.JobTitle.Name);
            }
        }

        [Fact]
        public void ChangeEmployeeEffectiveRecord()
        {
            using (IKernel Kernel = new StandardKernel())
            {
                Kernel.Load(new HRMS.Core.Configure());
                Core.Models.Db.Employee Employee = TempEmployees.First();

                Core.Models.Employee NewEmployee = Kernel.Get<Core.Models.Employee>(new ConstructorArgument("EmployeeID", Employee.EmployeeID.Value));
                NewEmployee.Load();
                NewEmployee.ChangeEffectiveDate(new DateTime(2015, 1, 1));

                Core.Models.EmployeeRecord Record = NewEmployee.GetOrCreateEffectiveDateRange(new DateTime(2015, 1, 1));

                Assert.Equal(Record, NewEmployee.EffectiveRecord);
                Assert.Equal(2, NewEmployee.EmployeeRecords.Count());
                Assert.False(Record.Dirty);

                Record = NewEmployee.GetOrCreateEffectiveDateRange(new DateTime(2014, 1, 1));

                Assert.Equal(3, NewEmployee.EmployeeRecords.Count());
                Assert.True(Record.Dirty);
                Assert.Equal(new DateTime(2014, 1, 1), Record.EffectiveDate);
                Assert.Equal(new DateTime(2015, 1, 1), Record.EndEffectiveDate);
                Assert.Equal(0, Record.AllEntities.Count());

                NewEmployee = Kernel.Get<Core.Models.Employee>(new ConstructorArgument("EmployeeID", Employee.EmployeeID.Value));
                NewEmployee.Load();
                NewEmployee.ChangeEffectiveDate(new DateTime(2015, 1, 1));

                Record = NewEmployee.GetOrCreateEffectiveDateRange(new DateTime(2015, 1, 15));
                Core.Models.EmployeeRecord PreviousRecord = NewEmployee.EmployeeRecords.First().Value;

                Assert.Equal(3, NewEmployee.EmployeeRecords.Count());
                Assert.True(Record.Dirty);
                Assert.Equal(new DateTime(2015, 1, 15), Record.EffectiveDate);
                Assert.Equal(new DateTime(2015, 2, 1), Record.EndEffectiveDate);
                Assert.Equal(new DateTime(2015, 1, 1), PreviousRecord.EffectiveDate);
                Assert.Equal(new DateTime(2015, 1, 15), PreviousRecord.EndEffectiveDate);
                Assert.Equal(new DateTime(2015, 2, 1), NewEmployee.EmployeeRecords.Last().Value.EffectiveDate);
                Assert.Null(NewEmployee.EmployeeRecords.Last().Value.EndEffectiveDate);
                for (int i = 0; i < PreviousRecord.AllEntities.Count(); i++)
                {
                    Assert.Equal(PreviousRecord.AllEntities.ElementAt(i), Record.AllEntities.ElementAt(i));
                }

                NewEmployee = Kernel.Get<Core.Models.Employee>(new ConstructorArgument("EmployeeID", Employee.EmployeeID.Value));
                NewEmployee.Load();
                NewEmployee.ChangeEffectiveDate(new DateTime(2015, 1, 1));

                Record = NewEmployee.GetOrCreateEffectiveDateRange(new DateTime(2015, 3, 1));
                PreviousRecord = NewEmployee.EmployeeRecords.ElementAt(1).Value;

                Assert.Equal(3, NewEmployee.EmployeeRecords.Count());
                Assert.True(Record.Dirty);
                Assert.Equal(new DateTime(2015, 3, 1), Record.EffectiveDate);
                Assert.Null(Record.EndEffectiveDate);
                Assert.Equal(new DateTime(2015, 1, 1), NewEmployee.EmployeeRecords.First().Value.EffectiveDate);
                Assert.Equal(new DateTime(2015, 2, 1), NewEmployee.EmployeeRecords.First().Value.EndEffectiveDate);
                Assert.Equal(new DateTime(2015, 2, 1), PreviousRecord.EffectiveDate);
                Assert.Equal(new DateTime(2015, 3, 1), PreviousRecord.EndEffectiveDate);
                for (int i = 0; i < PreviousRecord.AllEntities.Count(); i++)
                {
                    Assert.Equal(PreviousRecord.AllEntities.ElementAt(i), Record.AllEntities.ElementAt(i));
                }
            }
        }

        public void Dispose()
        {
            TearDownEF7DatabaseIfRequired();
        }
    }
}
