﻿using System;
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
using EffectFramework.Core.Forms;
using EffectFramework.Core;
using Microsoft.Framework.ConfigurationModel;
using System.IO;
using EffectFramework.Core.Exceptions;

namespace EffectFramework.Test
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class DataModelTests : IDisposable
    {
        private List<Core.Models.Db.Item> TempItems = new List<Core.Models.Db.Item>();
        private List<Entity> TempEntity = new List<Entity>();
        private List<Field> TempField = new List<Field>();
        private bool DatabaseIsPrepared = false;

        private IConfiguration Configuration { get; set; }

        private string _BasePath = null;
        private string BasePath
        {
            get
            {
                if (_BasePath == null)
                {
                    _BasePath = Directory.GetCurrentDirectory();
                }
                return _BasePath;
            }
        }

        private void PrepareEF7DatabaseIfRequired()
        {
            if (!DatabaseIsPrepared)
            {
                using (var db = new ItemDb7Context(Configuration["Data:DefaultConnection:ConnectionString"]))
                using (db.BeginTransaction())
                {
                    Core.Models.Db.Item NewItem = new Core.Models.Db.Item()
                    {
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
                        EntityTypeID = TestEntityType.General_Info,
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

                    Field NewField1 = new Field()
                    {
                        EntityID = NewEntity1.EntityID,
                        FieldTypeID = TestFieldType.Hire_Date.Value,
                        IsDeleted = false,
                        ValueDate = new DateTime(2015, 1, 1),
                        Guid = Guid.NewGuid(),
                    };

                    Field NewField4 = new Field()
                    {
                        EntityID = NewEntity1.EntityID,
                        FieldTypeID = TestFieldType.First_Name.Value,
                        IsDeleted = false,
                        ValueText = "John",
                        Guid = Guid.NewGuid(),
                    };

                    Field NewField5 = new Field()
                    {
                        EntityID = NewEntity1.EntityID,
                        FieldTypeID = TestFieldType.Last_Name.Value,
                        IsDeleted = false,
                        ValueText = "Smith",
                        Guid = Guid.NewGuid(),
                    };

                    Field NewField2 = new Field()
                    {
                        EntityID = NewEntity2.EntityID,
                        FieldTypeID = TestFieldType.Job_Title.Value,
                        IsDeleted = false,
                        ValueText = "Manager",
                        Guid = Guid.NewGuid(),
                    };

                    Field NewField3 = new Field()
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
                    db.Fields.Add(NewField4);
                    db.Fields.Add(NewField5);
                    db.SaveChanges();

                    db.Database.AsRelational().Connection.Transaction.Commit();

                    DatabaseIsPrepared = true;

                    TempItems.Add(NewItem);
                }
            }
        }

        private void TearDownEF7DatabaseIfRequired()
        {
            if (DatabaseIsPrepared)
            {
                using (var db = new ItemDb7Context(Configuration["Data:DefaultConnection:ConnectionString"]))
                using (db.Database.AsRelational().Connection.BeginTransaction())
                {
                    var AllEntities = db.Entities.Where(e => e.ItemID == TempItems.First().ItemID).ToList();
                    var AllFields = db.Fields.Where(f => AllEntities.Select(e => e.EntityID).Contains(f.EntityID)).ToList();

                    foreach (var Field in AllFields)
                    {
                        db.Fields.Remove(Field);
                    }
                    db.SaveChanges();

                    foreach (var Entity in AllEntities)
                    {
                        db.Entities.Remove(Entity);
                    }
                    db.SaveChanges();

                    foreach (var Item in TempItems)
                    {
                        db.Items.Remove(Item);
                    }
                    db.SaveChanges();

                    db.Database.AsRelational().Connection.Transaction.Commit();

                    TempField.Clear();
                    TempItems.Clear();
                    TempEntity.Clear();
                }
            }
        }

        public DataModelTests()
        {
            var configuration = new Configuration(BasePath)
                .AddJsonFile("config.json");
            Configuration = configuration;

            Configure.ConnectionString = Configuration["Data:DefaultConnection:ConnectionString"];

            PrepareEF7DatabaseIfRequired();
        }

        [Fact]
        public void CreateEF7DBContext()
        {
            using (var db = new ItemDb7Context(Configuration["Data:DefaultConnection:ConnectionString"]))
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
                NewEmployee.EffectiveDate = new DateTime(2015, 1, 1);

                Assert.Equal(Item.ItemID, NewEmployee.ItemID);
                Assert.Equal(Item.ItemTypeID, NewEmployee.Type.Value);
                Assert.Equal(3, NewEmployee.AllEntities.Count());
                Assert.Equal(2, NewEmployee.EffectiveRecord.AllEntities.Count());
                Assert.False(NewEmployee.EffectiveRecord.AllEntities.First().Dirty);
                Assert.False(NewEmployee.EffectiveRecord.AllEntities.Last().Dirty);
                Assert.False(NewEmployee.Dirty);

                var Record = NewEmployee.GetEntityCollectionForDate(new DateTime(2015, 2, 1));

                Assert.Equal(2, Record.AllEntities.Count());
                Assert.False(Record.AllEntities.First().Dirty);
                Assert.False(Record.AllEntities.Last().Dirty);
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
                NewEmployee.EffectiveDate = new DateTime(2015, 1, 1);

                Assert.False(NewEmployee.Dirty);
                Assert.Equal(Item.Guid, NewEmployee.Guid);

                GeneralInfoEntity GeneralEntity = NewEmployee.EffectiveRecord.GetFirstEntityOrDefault<GeneralInfoEntity>();

                Assert.NotNull(GeneralEntity);
                Assert.Equal(new DateTime(2015, 1, 1), GeneralEntity.HireDate.Value);

                JobEntity FirstJob = NewEmployee.EffectiveRecord.GetFirstEntityOrDefault<JobEntity>();

                Assert.NotNull(FirstJob);
                Assert.Equal("Manager", FirstJob.JobTitle.Value);

                NewEmployee.EffectiveDate = new DateTime(2015, 2, 2);

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
                NewEmployee.EffectiveDate = new DateTime(2015, 1, 1);

                GeneralInfoEntity GeneralEntity = NewEmployee.EffectiveRecord.GetFirstEntityOrDefault<GeneralInfoEntity>();

                Assert.NotNull(GeneralEntity);
                Assert.NotNull(GeneralEntity.EntityID);
                Assert.NotEqual(Guid.Empty, GeneralEntity.Guid);
                Assert.Equal(new DateTime(2015, 1, 1), GeneralEntity.EffectiveDate);
                Assert.Null(GeneralEntity.EndEffectiveDate);
                Assert.False(GeneralEntity.Dirty);
                Assert.Equal(Strings.General_Info, GeneralEntity.Type.Name);

                JobEntity FirstJob = NewEmployee.EffectiveRecord.GetFirstEntityOrDefault<JobEntity>();

                Assert.NotNull(FirstJob);
                Assert.NotNull(FirstJob.EntityID);
                Assert.NotEqual(Guid.Empty, FirstJob.Guid);
                Assert.Equal(new DateTime(2015, 1, 1), FirstJob.EffectiveDate);
                Assert.Equal(new DateTime(2015, 2, 1), FirstJob.EndEffectiveDate);
                Assert.False(FirstJob.Dirty);
                Assert.Equal(Strings.Job, FirstJob.Type.Name);

                NewEmployee.EffectiveDate = new DateTime(2015, 2, 2);

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
                NewEmployee.EffectiveDate = new DateTime(2015, 1, 1);

                GeneralInfoEntity GeneralEntity = NewEmployee.EffectiveRecord.GetFirstEntityOrDefault<GeneralInfoEntity>();

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

                NewEmployee.EffectiveDate = new DateTime(2015, 2, 2);

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
        public void CreateNewEntity()
        {
            using (IKernel Kernel = new StandardKernel(new Configure()))
            {
                Core.Models.Db.Item Item = TempItems.First();

                Employee NewEmployee = Kernel.Get<Employee>(new ConstructorArgument("EmployeeID", Item.ItemID.Value));
                NewEmployee.EffectiveDate = new DateTime(2015, 1, 1);

                var Address = NewEmployee.EffectiveRecord.GetOrCreateEntity<AddressEntity>();

                Assert.NotNull(Address);
                Assert.True(Address.Dirty);
                Assert.False(Address.EntityID.HasValue);
                Assert.Equal(NewEmployee.EffectiveRecord.EffectiveDate, Address.EffectiveDate);
                
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
                NewEmployee.EffectiveDate = new DateTime(2015, 1, 1);

                FieldString JobTitle = NewEmployee.EffectiveRecord.GetFirstEntityOrDefault<JobEntity>().JobTitle;

                Assert.False(JobTitle.Dirty);

                JobTitle.Value = "Code Monkey";

                Assert.True(JobTitle.Dirty);

                JobTitle.PersistToDatabase();

                Assert.False(JobTitle.Dirty);

                using (var db = new ItemDb7Context(Configuration["Data:DefaultConnection:ConnectionString"]))
                {
                    Field Field = db.Fields.Where(ef => ef.FieldID == JobTitle.FieldID.Value).Single();

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
                NewEmployee.EffectiveDate = new DateTime(2015, 1, 1);

                JobEntity Job = NewEmployee.EffectiveRecord.GetFirstEntityOrDefault<JobEntity>();
                Job.JobStartDate.Value = new DateTime(2015, 1, 1);

                Job.PersistToDatabase();

                using (var db = new ItemDb7Context(Configuration["Data:DefaultConnection:ConnectionString"]))
                {
                    TempField.Add(db.Fields.Where(f => f.FieldID == Job.JobStartDate.FieldID.Value).Single());
                }

                NewEmployee = Kernel.Get<Employee>(new ConstructorArgument("EmployeeID", Item.ItemID.Value));
                NewEmployee.EffectiveDate = new DateTime(2015, 1, 1);

                Job = NewEmployee.EffectiveRecord.GetFirstEntityOrDefault<JobEntity>();
                Assert.Equal(new DateTime(2015, 1, 1), Job.JobStartDate.Value);

            }
        }

        [Fact]
        public void PopulateForm()
        {
            Employee Employee = null;
            using (IKernel Kernel = new StandardKernel(new Configure()))
            {
                Employee = Kernel.Get<Employee>(new ConstructorArgument("EmployeeID", TempItems.First().ItemID));
            }

            Employee.EffectiveDate = new DateTime(2015, 3, 1);
            GeneralInfoEntity Entity = Employee.EffectiveRecord.CreateEntityAndEndDateAllPrevious<GeneralInfoEntity>(CopyValuesFromPrevious: true);
            Entity.First_Name.Value = "Bobby";
            Employee.PersistToDatabase();

            using (IKernel Kernel = new StandardKernel(new Configure()))
            {
                Employee = Kernel.Get<Employee>(new ConstructorArgument("EmployeeID", TempItems.First().ItemID));
            }
            Employee.EffectiveDate = new DateTime(2015, 1, 1);
            GeneralInfoForm Form = new GeneralInfoForm()
            {
                GeneralInfoID = Employee.EffectiveRecord.GetFirstEntityOrDefault<GeneralInfoEntity>().EntityID,
            };
            Form.BindTo(Employee);
            Form.PopulateForm();

            Entity = Employee.EffectiveRecord.GetFirstEntityOrDefault<GeneralInfoEntity>();

            Assert.NotNull(Entity);
            Assert.Equal(Entity.First_Name.Value, Form.First_Name);
            Assert.Equal(Entity.Last_Name.Value, Form.Last_Name);
            Assert.Equal(Entity.EntityID.Value, Form.GeneralInfoID);

            using (IKernel Kernel = new StandardKernel(new Configure()))
            {
                Employee = Kernel.Get<Employee>(new ConstructorArgument("EmployeeID", TempItems.First().ItemID));
            }
            Employee.EffectiveDate = new DateTime(2015, 1, 1);
            Form = new GeneralInfoForm();
            Form.BindTo(Employee);
            Form.PopulateForm();

            Entity = Employee.EffectiveRecord.GetFirstEntityOrDefault<GeneralInfoEntity>();

            Assert.NotNull(Entity);
            Assert.Equal(Entity.First_Name.Value, Form.First_Name);
            Assert.Equal(Entity.Last_Name.Value, Form.Last_Name);
            Assert.Equal(Entity.EntityID.Value, Form.GeneralInfoID);

            using (IKernel Kernel = new StandardKernel(new Configure()))
            {
                Employee = Kernel.Get<Employee>(new ConstructorArgument("EmployeeID", TempItems.First().ItemID));
            }
            Employee.EffectiveDate = new DateTime(2015, 3, 1);
            Form = new GeneralInfoForm()
            {
                GeneralInfoID = Employee.EffectiveRecord.GetFirstEntityOrDefault<GeneralInfoEntity>().EntityID,
                Effective_Date = new DateTime(2015, 3, 1),
            };
            Form.BindTo(Employee);
            Form.PopulateForm();

            Entity = Employee.EffectiveRecord.GetFirstEntityOrDefault<GeneralInfoEntity>();

            Assert.NotNull(Entity);
            Assert.Equal(Entity.First_Name.Value, Form.First_Name);
            Assert.Equal(Entity.Last_Name.Value, Form.Last_Name);
            Assert.Equal(Entity.EntityID.Value, Form.GeneralInfoID);
            Assert.Equal("Bobby", Form.First_Name);
        }

        [Fact]
        public void PersistForm()
        {
            Employee Employee = null;

            using (IKernel Kernel = new StandardKernel(new Configure()))
            {
                Employee = Kernel.Get<Employee>(new ConstructorArgument("EmployeeID", TempItems.First().ItemID));
            }
            Employee.EffectiveDate = new DateTime(2015, 1, 1);
            GeneralInfoForm Form = new GeneralInfoForm();
            Form.BindTo(Employee);
            Form.PopulateForm();

            Form.Effective_Date = new DateTime(2015, 4, 1);
            Form.GeneralInfoID = null;
            Form.First_Name = "Johnny";
            Form.Last_Name = "Jones";
            Form.PushValuesToModel();

            Employee.PersistToDatabase();

            using (IKernel Kernel = new StandardKernel(new Configure()))
            {
                Employee = Kernel.Get<Employee>(new ConstructorArgument("EmployeeID", TempItems.First().ItemID));
            }
            Employee.EffectiveDate = new DateTime(2015, 4, 1);
            var Entity = Employee.EffectiveRecord.GetFirstEntityOrDefault<GeneralInfoEntity>();

            Assert.NotNull(Entity);
            Assert.Equal(Form.First_Name, Entity.First_Name.Value);
            Assert.Equal(Form.Last_Name, Entity.Last_Name.Value);

            Employee.EffectiveDate = new DateTime(2015, 1, 1);

            Entity = Employee.EffectiveRecord.GetFirstEntityOrDefault<GeneralInfoEntity>();
            Assert.NotNull(Entity);
            Assert.Equal("John", Entity.First_Name.Value);
            Assert.Equal("Smith", Entity.Last_Name.Value);

        }
        
        [Fact]
        public void EnsureConcurrentUpdatesDontOverwriteEachOther()
        {
            using (IKernel Kernel = new StandardKernel(new Configure()))
            {
                Employee Employee1 = Kernel.Get<Employee>(new ConstructorArgument("EmployeeID", TempItems.First().ItemID));
                Employee Employee2 = Kernel.Get<Employee>(new ConstructorArgument("EmployeeID", TempItems.First().ItemID));

                Employee1.EffectiveRecord.GetFirstEntityOrDefault<JobEntity>().JobTitle.Value = "CEO";
                Employee2.EffectiveRecord.GetFirstEntityOrDefault<JobEntity>().JobTitle.Value = "Janitor";

                Employee1.PersistToDatabase();
                Assert.Throws(typeof(GuidMismatchException), () => { Employee2.PersistToDatabase(); });

                Employee2.Load();
                Assert.Equal("CEO", Employee2.EffectiveRecord.GetFirstEntityOrDefault<JobEntity>().JobTitle.Value);
            }
        }

        [Fact]
        public void EnsureButtSplicedEntitiesDontPersist()
        {
            using (IKernel Kernel = new StandardKernel(new Configure()))
            {
                Employee Employee1 = Kernel.Get<Employee>(new ConstructorArgument("EmployeeID", TempItems.First().ItemID));

                Employee1.EffectiveRecord.CreateEntityAndEndDateAllPrevious<GeneralInfoEntity>(true);

                Employee1.PersistToDatabase();

                Assert.Equal(1, Employee1.AllEntities.Where(e => e.Type == TestEntityType.General_Info).Count());
                Employee1.Load();

                Assert.Equal(1, Employee1.AllEntities.Where(e => e.Type == TestEntityType.General_Info).Count());

                Employee1.EffectiveRecord.CreateEntityAndEndDateAllPrevious<GeneralInfoEntity>(true, new DateTime(2015,9,1));
                Employee1.PersistToDatabase();

                Assert.Equal(1, Employee1.AllEntities.Where(e => e.Type == TestEntityType.General_Info).Count());
                Employee1.Load();

                Assert.Equal(1, Employee1.AllEntities.Where(e => e.Type == TestEntityType.General_Info).Count());
                Assert.Equal(new DateTime(2015, 9, 1), Employee1.AllEntities.Where(e => e.Type == TestEntityType.General_Info).First().EndEffectiveDate);
            }
        }

        public void Dispose()
        {
            //TearDownEF7DatabaseIfRequired();
        }
    }

#region Entities And Fields And Forms
    [Bind(typeof(Employee), typeof(GeneralInfoEntity), "GeneralInfoID")]
    public class GeneralInfoForm : Form
    {
        [Bind]
        public string First_Name { get; set; }
        [Bind]
        public string Last_Name { get; set; }

        [EffectiveDate]
        public DateTime Effective_Date { get; set; }
        public int? GeneralInfoID { get; set; }
    }
    public class TestEntityType : EntityType
    {
        public static readonly TestEntityType Job = new TestEntityType(Strings.Job, 1, typeof(JobEntity));
        public static readonly TestEntityType Address = new TestEntityType(Strings.Address, 2, typeof(AddressEntity));
        public static readonly TestEntityType General_Info = new TestEntityType(Strings.General_Info, 3, typeof(GeneralInfoEntity));

        protected TestEntityType(string Name, int Value, Type Type) : base(Name, Value, Type) { }
    }

    public class TestFieldType : FieldType
    {
        protected TestFieldType(string Name, int Value, DataType DataType) : base(Name, Value, DataType) { }

        public static readonly TestFieldType Job_Title = new TestFieldType(Strings.Job_Title, 1, DataType.Text);
        public static readonly TestFieldType Job_Start_Date = new TestFieldType(Strings.Job_Start_Date, 2, DataType.Date);
        public static readonly TestFieldType Hire_Date = new TestFieldType(Strings.Hire_Date, 3, DataType.Date);
        public static readonly TestFieldType First_Name = new TestFieldType(Strings.First_Name, 4, DataType.Text);
        public static readonly TestFieldType Last_Name = new TestFieldType(Strings.Last_Name, 5, DataType.Text);

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

    public class GeneralInfoEntity : EntityBase
    {
        public override EntityType Type
        {
            get
            {
                return TestEntityType.General_Info;
            }
        }

        public GeneralInfoEntity() : base()
        {

        }



        public GeneralInfoEntity(IPersistenceService PersistenceService)
            : base(PersistenceService)
        {
        }

        protected override void WireUpFields()
        {
            HireDate = new FieldDate(TestFieldType.Hire_Date, PersistenceService);
            First_Name = new FieldString(TestFieldType.First_Name, PersistenceService);
            Last_Name = new FieldString(TestFieldType.Last_Name, PersistenceService);
        }

        public FieldDate HireDate { get; private set; }
        public FieldString First_Name { get; private set; }
        public FieldString Last_Name { get; private set; }
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