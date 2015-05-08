using System;
using System.Collections.Generic;
using System.Linq;
using EffectFramework.Core.Models.Db;
using Xunit;
using Ninject;
using EffectFramework.Core.Models.Entities;
using Microsoft.Data.Entity;
using EffectFramework.Core.Models.Fields;
using Ninject.Parameters;
using EffectFramework.Core.Services;
using EffectFramework.Core.Models;

namespace EffectFramework.Test
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class DataModelTests : IDisposable
    {
        private List<Core.Models.Db.Item> TempItems = new List<Core.Models.Db.Item>();
        private List<Entity> TempEntity = new List<Entity>();
        private List<Core.Models.Db.ItemRecord> TempItemRecord = new List<Core.Models.Db.ItemRecord>();
        private List<ItemEntity> TempItemEntity = new List<ItemEntity>();
        private List<EntityField> TempEntityField = new List<EntityField>();
        private bool DatabaseIsPrepared = false;

        private void PrepareEF7DatabaseIfRequired()
        {
            if (!DatabaseIsPrepared)
            {
                using (var db = new ItemDb7Context())
                using (db.Database.AsRelational().Connection.BeginTransaction())
                {
                    Core.Models.Db.Item NewItem = new Core.Models.Db.Item()
                    {
                        DisplayName = "xUnit Test Item",
                        ItemTypeID = TestItemType.Employee.Value,
                        IsDeleted = false,
                        Guid = Guid.NewGuid(),
                    };

                    db.Items.Add(NewItem);
                    db.SaveChanges();

                    Entity NewEntity1 = new Entity()
                    {
                        EffectiveDate = new DateTime(2015, 1, 1),
                        EndEffectiveDate = null,
                        ItemID = NewItem.ItemID.Value,
                        EntityTypeID = TestEntityType.Employee_General,
                        IsDeleted = false,
                        Guid = Guid.NewGuid(),
                    };


                    Entity NewEntity2 = new Entity()
                    {
                        EffectiveDate = new DateTime(2015, 1, 1),
                        EndEffectiveDate = new DateTime(2015, 2, 1),
                        ItemID = NewItem.ItemID.Value,
                        EntityTypeID = TestEntityType.Job,
                        IsDeleted = false,
                        Guid = Guid.NewGuid(),
                    };

                    Entity NewEntity3 = new Entity()
                    {
                        EffectiveDate = new DateTime(2015, 2, 1),
                        EndEffectiveDate = null,
                        ItemID = NewItem.ItemID.Value,
                        EntityTypeID = TestEntityType.Job,
                        IsDeleted = false,
                        Guid = Guid.NewGuid(),
                    };

                    db.Entities.Add(NewEntity1);
                    db.Entities.Add(NewEntity2);
                    db.Entities.Add(NewEntity3);
                    db.SaveChanges();

                    Core.Models.Db.ItemRecord NewItemRecord1 = new Core.Models.Db.ItemRecord()
                    {
                        EffectiveDate = new DateTime(2015, 1, 1),
                        EndEffectiveDate = new DateTime(2015, 2, 1),
                        ItemID = NewItem.ItemID.Value,
                        IsDeleted = false,
                        Guid = Guid.NewGuid(),
                    };

                    Core.Models.Db.ItemRecord NewItemRecord2 = new Core.Models.Db.ItemRecord()
                    {
                        EffectiveDate = new DateTime(2015, 2, 1),
                        EndEffectiveDate = null,
                        ItemID = NewItem.ItemID.Value,
                        IsDeleted = false,
                        Guid = Guid.NewGuid(),
                    };

                    db.ItemRecords.Add(NewItemRecord1);
                    db.ItemRecords.Add(NewItemRecord2);
                    db.SaveChanges();

                    NewItem.ItemRecordID = NewItemRecord2.ItemRecordID;
                    db.SaveChanges();

                    ItemEntity NewItemEntity1 = new ItemEntity()
                    {
                        ItemRecordID = NewItemRecord1.ItemRecordID.Value,
                        EntityID = NewEntity1.EntityID,
                        IsDeleted = false,
                        Guid = Guid.NewGuid(),
                    };

                    ItemEntity NewItemEntity2 = new ItemEntity()
                    {
                        ItemRecordID = NewItemRecord1.ItemRecordID.Value,
                        EntityID = NewEntity2.EntityID,
                        IsDeleted = false,
                        Guid = Guid.NewGuid(),
                    };

                    ItemEntity NewItemEntity3 = new ItemEntity()
                    {
                        ItemRecordID = NewItemRecord2.ItemRecordID.Value,
                        EntityID = NewEntity1.EntityID,
                        IsDeleted = false,
                        Guid = Guid.NewGuid(),
                    };

                    ItemEntity NewItemEntity4 = new ItemEntity()
                    {
                        ItemRecordID = NewItemRecord2.ItemRecordID.Value,
                        EntityID = NewEntity3.EntityID,
                        IsDeleted = false,
                        Guid = Guid.NewGuid(),
                    };

                    db.ItemEntities.Add(NewItemEntity1);
                    db.ItemEntities.Add(NewItemEntity2);
                    db.ItemEntities.Add(NewItemEntity3);
                    db.ItemEntities.Add(NewItemEntity4);
                    db.SaveChanges();

                    EntityField NewField1 = new EntityField()
                    {
                        EntityID = NewEntity1.EntityID,
                        FieldTypeID = TestFieldType.Hire_Date.Value,
                        IsDeleted = false,
                        ValueDate = new DateTime(2015, 1, 1),
                        Guid = Guid.NewGuid(),
                    };

                    EntityField NewField2 = new EntityField()
                    {
                        EntityID = NewEntity2.EntityID,
                        FieldTypeID = TestFieldType.Job_Title.Value,
                        IsDeleted = false,
                        ValueText = "Manager",
                        Guid = Guid.NewGuid(),
                    };

                    EntityField NewField3 = new EntityField()
                    {
                        EntityID = NewEntity3.EntityID,
                        FieldTypeID = TestFieldType.Job_Title.Value,
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

                    TempItems.Add(NewItem);
                    TempEntity.Add(NewEntity1);
                    TempEntity.Add(NewEntity2);
                    TempEntity.Add(NewEntity3);
                    TempItemRecord.Add(NewItemRecord1);
                    TempItemRecord.Add(NewItemRecord2);
                    TempItemEntity.Add(NewItemEntity1);
                    TempItemEntity.Add(NewItemEntity2);
                    TempItemEntity.Add(NewItemEntity3);
                    TempItemEntity.Add(NewItemEntity4);
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
                using (var db = new ItemDb7Context())
                using (db.Database.AsRelational().Connection.BeginTransaction())
                {
                    foreach (var Field in TempEntityField)
                    {
                        db.Fields.Remove(Field);
                    }
                    db.SaveChanges();

                    foreach (var Item in TempItems)
                    {
                        Item.ItemRecordID = null;
                    }
                    db.SaveChanges();

                    var Command = db.Database.AsSqlServer().Connection.DbConnection.CreateCommand();
                    Command.CommandText = @"
                            ALTER TABLE ItemRecords NOCHECK CONSTRAINT all;
                            ALTER TABLE ItemEntities NOCHECK CONSTRAINT all;
                            ALTER TABLE Items NOCHECK CONSTRAINT all;";
                    Command.Transaction = db.Database.AsRelational().Connection.DbTransaction;
                    Command.ExecuteNonQuery();

                    foreach (var ItemRecord in TempItemRecord)
                    {
                        db.ItemRecords.Remove(ItemRecord);
                    }
                    db.SaveChanges();

                    foreach (var ItemEntity in TempItemEntity)
                    {
                        db.ItemEntities.Remove(ItemEntity);
                    }
                    db.SaveChanges();

                    foreach (var Entity in TempEntity)
                    {
                        db.Entities.Remove(Entity);
                    }
                    db.SaveChanges();

                    foreach (var Item in TempItems)
                    {
                        db.Items.Remove(Item);
                    }
                    db.SaveChanges();

                    var Command2 = db.Database.AsSqlServer().Connection.DbConnection.CreateCommand();
                    Command2.CommandText = @"
                            ALTER TABLE ItemRecords WITH CHECK CHECK CONSTRAINT all;
                            ALTER TABLE ItemEntities WITH CHECK CHECK CONSTRAINT all;
                            ALTER TABLE Items WITH CHECK CHECK CONSTRAINT all;";
                    Command2.Transaction = db.Database.AsRelational().Connection.DbTransaction;
                    Command2.ExecuteNonQuery();

                    db.Database.AsRelational().Connection.Transaction.Commit();

                    TempEntityField.Clear();
                    TempItems.Clear();
                    TempItemRecord.Clear();
                    TempItemEntity.Clear();
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
            using (var db = new ItemDb7Context())
            {

            }
        }

        [Fact]
        public void LoadUpItemObject()
        {
            using (IKernel Kernel = new StandardKernel())
            {
                Kernel.Load(new EffectFramework.Core.Configure());

                Core.Models.Db.Item Item = TempItems.First();

                Employee NewEmployee = Kernel.Get<Employee>(new ConstructorArgument("EmployeeID", Item.ItemID.Value));
                NewEmployee.ChangeEffectiveDate(new DateTime(2015, 1, 1));

                Assert.Equal(Item.ItemID, NewEmployee.ItemID);
                Assert.Equal(Item.ItemTypeID, NewEmployee.Type.Value);
                Assert.Equal(2, NewEmployee.ItemRecords.Count());
                Assert.Equal(2, NewEmployee.ItemRecords.First().Value.AllEntities.Count());
                Assert.Equal(2, NewEmployee.ItemRecords.Last().Value.AllEntities.Count());
                Assert.False(NewEmployee.ItemRecords.First().Value.Dirty);
                Assert.False(NewEmployee.ItemRecords.Last().Value.Dirty);

            }
        }

        [Fact]
        public void RetreiveEntitiesByEffectiveDate()
        {
            using (IKernel Kernel = new StandardKernel())
            {
                Kernel.Load(new EffectFramework.Core.Configure());
                Core.Models.Db.Item Item = TempItems.First();

                Employee NewEmployee = Kernel.Get<Employee>(new ConstructorArgument("EmployeeID", Item.ItemID.Value));
                NewEmployee.ChangeEffectiveDate(new DateTime(2015, 1, 1));

                Assert.False(NewEmployee.Dirty);
                Assert.Equal(Item.Guid, NewEmployee.Guid);

                EmployeeGeneralEntity GeneralEntity = NewEmployee.ItemRecords.First().Value.GetFirstEntityOrDefault<EmployeeGeneralEntity>();

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
                Kernel.Load(new EffectFramework.Core.Configure());
                Core.Models.Db.Item Item = TempItems.First();

                Employee NewEmployee = Kernel.Get<Employee>(new ConstructorArgument("EmployeeID", Item.ItemID.Value));
                NewEmployee.ChangeEffectiveDate(new DateTime(2015, 1, 1));

                EmployeeGeneralEntity GeneralEntity = NewEmployee.ItemRecords.First().Value.GetFirstEntityOrDefault<EmployeeGeneralEntity>();

                Assert.NotNull(GeneralEntity);
                Assert.NotNull(GeneralEntity.EntityID);
                Assert.NotEqual(Guid.Empty, GeneralEntity.Guid);
                Assert.Equal(new DateTime(2015, 1, 1), GeneralEntity.EffectiveDate);
                Assert.Null(GeneralEntity.EndEffectiveDate);
                Assert.False(GeneralEntity.Dirty);
                Assert.Equal(Strings.Employee_General, GeneralEntity.Type.Name);

                JobEntity FirstJob = NewEmployee.EffectiveRecord.GetFirstEntityOrDefault<JobEntity>();

                Assert.NotNull(FirstJob);
                Assert.NotNull(FirstJob.EntityID);
                Assert.NotEqual(Guid.Empty, FirstJob.Guid);
                Assert.Equal(new DateTime(2015, 1, 1), FirstJob.EffectiveDate);
                Assert.Equal(new DateTime(2015, 2, 1), FirstJob.EndEffectiveDate);
                Assert.False(FirstJob.Dirty);
                Assert.Equal(Strings.Job, FirstJob.Type.Name);

                NewEmployee.ChangeEffectiveDate(new DateTime(2015, 2, 2));

                JobEntity SecondJob = NewEmployee.EffectiveRecord.GetFirstEntityOrDefault<JobEntity>();

                Assert.NotNull(SecondJob);
                Assert.NotNull(SecondJob.EntityID);
                Assert.NotEqual(Guid.Empty, SecondJob.Guid);
                Assert.Equal(new DateTime(2015, 2, 1), SecondJob.EffectiveDate);
                Assert.Null(SecondJob.EndEffectiveDate);
                Assert.False(SecondJob.Dirty);
                Assert.Equal(Strings.Job, SecondJob.Type.Name);
            }
        }

        [Fact]
        public void MakeSureFieldFieldsArePopulated()
        {
            using (IKernel Kernel = new StandardKernel())
            {
                Kernel.Load(new EffectFramework.Core.Configure());
                Core.Models.Db.Item Item = TempItems.First();

                Employee NewEmployee = Kernel.Get<Employee>(new ConstructorArgument("EmployeeID", Item.ItemID.Value));
                NewEmployee.ChangeEffectiveDate(new DateTime(2015, 1, 1));

                EmployeeGeneralEntity GeneralEntity = NewEmployee.ItemRecords.First().Value.GetFirstEntityOrDefault<EmployeeGeneralEntity>();

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
        public void ChangeItemEffectiveRecord()
        {
            using (IKernel Kernel = new StandardKernel())
            {
                Kernel.Load(new EffectFramework.Core.Configure());
                Core.Models.Db.Item Item = TempItems.First();

                Employee NewEmployee = Kernel.Get<Employee>(new ConstructorArgument("EmployeeID", Item.ItemID.Value));
                NewEmployee.ChangeEffectiveDate(new DateTime(2015, 1, 1));

                Core.Models.ItemRecord Record = NewEmployee.GetOrCreateEffectiveDateRange(new DateTime(2015, 1, 1));

                Assert.Equal(Record, NewEmployee.EffectiveRecord);
                Assert.Equal(2, NewEmployee.ItemRecords.Count());
                Assert.False(Record.Dirty);

                Record = NewEmployee.GetOrCreateEffectiveDateRange(new DateTime(2014, 1, 1));

                Assert.Equal(3, NewEmployee.ItemRecords.Count());
                Assert.True(Record.Dirty);
                Assert.Equal(new DateTime(2014, 1, 1), Record.EffectiveDate);
                Assert.Equal(new DateTime(2015, 1, 1), Record.EndEffectiveDate);
                Assert.Equal(0, Record.AllEntities.Count());

                NewEmployee = Kernel.Get<Employee>(new ConstructorArgument("EmployeeID", Item.ItemID.Value));
                NewEmployee.ChangeEffectiveDate(new DateTime(2015, 1, 1));

                Record = NewEmployee.GetOrCreateEffectiveDateRange(new DateTime(2015, 1, 15));
                Core.Models.ItemRecord PreviousRecord = NewEmployee.ItemRecords.First().Value;

                Assert.Equal(3, NewEmployee.ItemRecords.Count());
                Assert.True(Record.Dirty);
                Assert.Equal(new DateTime(2015, 1, 15), Record.EffectiveDate);
                Assert.Equal(new DateTime(2015, 2, 1), Record.EndEffectiveDate);
                Assert.Equal(new DateTime(2015, 1, 1), PreviousRecord.EffectiveDate);
                Assert.Equal(new DateTime(2015, 1, 15), PreviousRecord.EndEffectiveDate);
                Assert.Equal(new DateTime(2015, 2, 1), NewEmployee.ItemRecords.Last().Value.EffectiveDate);
                Assert.Null(NewEmployee.ItemRecords.Last().Value.EndEffectiveDate);
                for (int i = 0; i < PreviousRecord.AllEntities.Count(); i++)
                {
                    Assert.Equal(PreviousRecord.AllEntities.ElementAt(i), Record.AllEntities.ElementAt(i));
                }

                NewEmployee = Kernel.Get<Employee>(new ConstructorArgument("EmployeeID", Item.ItemID.Value));
                NewEmployee.ChangeEffectiveDate(new DateTime(2015, 1, 1));

                Record = NewEmployee.GetOrCreateEffectiveDateRange(new DateTime(2015, 3, 1));
                PreviousRecord = NewEmployee.ItemRecords.ElementAt(1).Value;

                Assert.Equal(3, NewEmployee.ItemRecords.Count());
                Assert.True(Record.Dirty);
                Assert.Equal(new DateTime(2015, 3, 1), Record.EffectiveDate);
                Assert.Null(Record.EndEffectiveDate);
                Assert.Equal(new DateTime(2015, 1, 1), NewEmployee.ItemRecords.First().Value.EffectiveDate);
                Assert.Equal(new DateTime(2015, 2, 1), NewEmployee.ItemRecords.First().Value.EndEffectiveDate);
                Assert.Equal(new DateTime(2015, 2, 1), PreviousRecord.EffectiveDate);
                Assert.Equal(new DateTime(2015, 3, 1), PreviousRecord.EndEffectiveDate);
                for (int i = 0; i < PreviousRecord.AllEntities.Count(); i++)
                {
                    Assert.Equal(PreviousRecord.AllEntities.ElementAt(i), Record.AllEntities.ElementAt(i));
                }
            }
        }

        [Fact]
        public void SavingFields()
        {
            using (IKernel Kernel = new StandardKernel())
            {
                Kernel.Load(new EffectFramework.Core.Configure());
                Core.Models.Db.Item Item = TempItems.First();

                Employee NewEmployee = Kernel.Get<Employee>(new ConstructorArgument("EmployeeID", Item.ItemID.Value));
                NewEmployee.ChangeEffectiveDate(new DateTime(2015, 1, 1));

                FieldString JobTitle = NewEmployee.EffectiveRecord.GetFirstEntityOrDefault<JobEntity>().JobTitle;

                Assert.False(JobTitle.Dirty);

                JobTitle.Value = "Code Monkey";

                Assert.True(JobTitle.Dirty);

                JobTitle.PersistToDatabase();

                Assert.False(JobTitle.Dirty);

                using (var db = new ItemDb7Context())
                {
                    EntityField Field = db.Fields.Where(ef => ef.EntityFieldID == JobTitle.FieldID.Value).Single();

                    Assert.Equal(Field.Guid, JobTitle.Guid);
                    Assert.Equal("Code Monkey", JobTitle.Value);

                }

                JobTitle.Value = "Manager";

                Assert.True(JobTitle.Dirty);

                JobTitle.PersistToDatabase();

                Assert.False(JobTitle.Dirty);
            }
        }

        [Fact]
        public void SaveNewField()
        {
            using (IKernel Kernel = new StandardKernel())
            {
                Kernel.Load(new EffectFramework.Core.Configure());
                Core.Models.Db.Item Item = TempItems.First();

                Employee NewEmployee = Kernel.Get<Employee>(new ConstructorArgument("EmployeeID", Item.ItemID.Value));
                NewEmployee.ChangeEffectiveDate(new DateTime(2015, 1, 1));

                JobEntity Job = NewEmployee.EffectiveRecord.GetFirstEntityOrDefault<JobEntity>();
                Job.JobStartDate.Value = new DateTime(2015, 1, 1);

                Job.PersistEntityToDatabase();

                using (var db = new ItemDb7Context())
                {
                    TempEntityField.Add(db.Fields.Where(f => f.EntityFieldID == Job.JobStartDate.FieldID.Value).Single());
                }

                NewEmployee = Kernel.Get<Employee>(new ConstructorArgument("EmployeeID", Item.ItemID.Value));
                NewEmployee.ChangeEffectiveDate(new DateTime(2015, 1, 1));

                Job = NewEmployee.EffectiveRecord.GetFirstEntityOrDefault<JobEntity>();
                Assert.Equal(new DateTime(2015, 1, 1), Job.JobStartDate.Value);

            }
        }

        public void Dispose()
        {
            TearDownEF7DatabaseIfRequired();
        }
    }

#region Entities And Fields
    public class TestEntityType : EntityType
    {
        public static readonly TestEntityType Job = new TestEntityType(Strings.Job, 1, typeof(JobEntity));
        public static readonly TestEntityType Address = new TestEntityType(Strings.Address, 2, typeof(AddressEntity));
        public static readonly TestEntityType Employee_General = new TestEntityType(Strings.Employee_General, 3, typeof(EmployeeGeneralEntity));

        protected TestEntityType(string Name, int Value, Type Type) : base(Name, Value, Type) { }
    }

    public class TestFieldType : FieldType
    {
        protected TestFieldType(string Name, int Value, DataType DataType) : base(Name, Value, DataType) { }

        public static readonly TestFieldType Job_Title = new TestFieldType(Strings.Job_Title, 1, DataType.Text);
        public static readonly TestFieldType Job_Start_Date = new TestFieldType(Strings.Job_Start_Date, 2, DataType.Date);
        public static readonly TestFieldType Hire_Date = new TestFieldType(Strings.Hire_Date, 3, DataType.Date);

    }

    public class TestItemType : ItemType
    {
        protected TestItemType(string Name, int Value, Type Type) : base(Name, Value, Type) { }

        public static readonly TestItemType Employee = new TestItemType("Employee", 1, typeof(Employee));

    }

    public class Employee : Core.Models.Item
    {
        public override ItemType Type {
            get
            {
                return TestItemType.Employee;
            }
        }

        public Employee(IPersistenceService PersistenceService) : base(PersistenceService) { }

        public Employee(int EmployeeID, IPersistenceService PersistenceService, bool LoadItem = true) : base(EmployeeID, PersistenceService, LoadItem) { }
    }

    public class AddressEntity : EntityBase
    {
        public override EntityType Type
        {
            get
            {
                return TestEntityType.Address;
            }
        }

        public AddressEntity() : base() { }

        public AddressEntity(IPersistenceService PersistenceService)
            : base(PersistenceService)
        {

        }

        protected override void WireUpFields()
        {

        }
    }

    public class EmployeeGeneralEntity : EntityBase
    {
        public override EntityType Type
        {
            get
            {
                return TestEntityType.Employee_General;
            }
        }

        public EmployeeGeneralEntity() : base()
        {

        }



        public EmployeeGeneralEntity(IPersistenceService PersistenceService)
            : base(PersistenceService)
        {
        }

        protected override void WireUpFields()
        {
            HireDate = new FieldDate(TestFieldType.Hire_Date, PersistenceService);
        }

        public FieldDate HireDate { get; private set; }
    }

    public class JobEntity : EntityBase
    {

        public override EntityType Type
        {
            get
            {
                return TestEntityType.Job;
            }
        }

        public JobEntity() : base() { }

        public JobEntity(IPersistenceService PersistenceService)
            : base(PersistenceService)
        {

        }

        protected override void WireUpFields()
        {
            JobTitle = new FieldString(TestFieldType.Job_Title, PersistenceService);
            JobStartDate = new FieldDate(TestFieldType.Job_Start_Date, PersistenceService);
        }

        public FieldString JobTitle { get; private set; }
        public FieldDate JobStartDate { get; private set; }
    }
#endregion
}