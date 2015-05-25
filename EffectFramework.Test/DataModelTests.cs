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
using EffectFramework.Core.Forms;
using EffectFramework.Core;
using Microsoft.Framework.ConfigurationModel;
using System.IO;
using EffectFramework.Core.Exceptions;

namespace EffectFramework.Test
{
    public partial class EffectFrameworkTests
    {
        public class DataModelTests : IDisposable
        {

            public DataModelTests()
            {
                var configuration = new Configuration(BasePath)
                    .AddJsonFile("config.json");
                Configuration = configuration;

                Configure.ConnectionString = Configuration["Data:DefaultConnection:ConnectionString"];

                PrepareEF7Database();
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

                    User NewUser = Kernel.Get<User>(new ConstructorArgument("UserID", Item.ItemID.Value));
                    NewUser.EffectiveDate = new DateTime(2015, 1, 1);

                    Assert.Equal(Item.ItemID, NewUser.ItemID);
                    Assert.Equal(Item.ItemTypeID, NewUser.Type.Value);
                    Assert.Equal(3, NewUser.AllEntities.Count());
                    Assert.Equal(2, NewUser.EffectiveRecord.AllEntities.Count());
                    Assert.False(NewUser.EffectiveRecord.AllEntities.First().Dirty);
                    Assert.False(NewUser.EffectiveRecord.AllEntities.Last().Dirty);
                    Assert.False(NewUser.Dirty);

                    var Record = NewUser.GetEntityCollectionForDate(new DateTime(2015, 2, 1));

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

                    User NewUser = Kernel.Get<User>(new ConstructorArgument("UserID", Item.ItemID.Value));
                    NewUser.EffectiveDate = new DateTime(2015, 1, 1);

                    Assert.False(NewUser.Dirty);
                    Assert.Equal(Item.Guid, NewUser.Guid);

                    GeneralInfoEntity GeneralEntity = NewUser.EffectiveRecord.GetFirstEntityOrDefault<GeneralInfoEntity>();

                    Assert.NotNull(GeneralEntity);
                    Assert.Equal(new DateTime(2015, 1, 1), GeneralEntity.HireDate.Value);

                    UserTypeEntity FirstJob = NewUser.EffectiveRecord.GetFirstEntityOrDefault<UserTypeEntity>();

                    Assert.NotNull(FirstJob);
                    Assert.Equal("Subscriber", FirstJob.UserType.Value);

                    NewUser.EffectiveDate = new DateTime(2015, 2, 2);

                    UserTypeEntity SecondJob = NewUser.EffectiveRecord.GetFirstEntityOrDefault<UserTypeEntity>();

                    Assert.NotNull(SecondJob);
                    Assert.Equal("Editor", SecondJob.UserType.Value);
                }
            }

            [Fact]
            public void MakeSureEntityFieldsArePopulated()
            {
                using (IKernel Kernel = new StandardKernel())
                {
                    Kernel.Load(new EffectFramework.Core.Configure());
                    Core.Models.Db.Item Item = TempItems.First();

                    User NewUser = Kernel.Get<User>(new ConstructorArgument("UserID", Item.ItemID.Value));
                    NewUser.EffectiveDate = new DateTime(2015, 1, 1);

                    GeneralInfoEntity GeneralEntity = NewUser.EffectiveRecord.GetFirstEntityOrDefault<GeneralInfoEntity>();

                    Assert.NotNull(GeneralEntity);
                    Assert.NotNull(GeneralEntity.EntityID);
                    Assert.NotEqual(Guid.Empty, GeneralEntity.Guid);
                    Assert.Equal(new DateTime(2015, 1, 1), GeneralEntity.EffectiveDate);
                    Assert.Null(GeneralEntity.EndEffectiveDate);
                    Assert.False(GeneralEntity.Dirty);
                    Assert.Equal(Strings.General_Info, GeneralEntity.Type.Name);

                    UserTypeEntity FirstJob = NewUser.EffectiveRecord.GetFirstEntityOrDefault<UserTypeEntity>();

                    Assert.NotNull(FirstJob);
                    Assert.NotNull(FirstJob.EntityID);
                    Assert.NotEqual(Guid.Empty, FirstJob.Guid);
                    Assert.Equal(new DateTime(2015, 1, 1), FirstJob.EffectiveDate);
                    Assert.Equal(new DateTime(2015, 2, 1), FirstJob.EndEffectiveDate);
                    Assert.False(FirstJob.Dirty);
                    Assert.Equal(Strings.Job, FirstJob.Type.Name);

                    NewUser.EffectiveDate = new DateTime(2015, 2, 2);

                    UserTypeEntity SecondJob = NewUser.EffectiveRecord.GetFirstEntityOrDefault<UserTypeEntity>();

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

                    User NewUser = Kernel.Get<User>(new ConstructorArgument("UserID", Item.ItemID.Value));
                    NewUser.EffectiveDate = new DateTime(2015, 1, 1);

                    GeneralInfoEntity GeneralEntity = NewUser.EffectiveRecord.GetFirstEntityOrDefault<GeneralInfoEntity>();

                    Assert.NotNull(GeneralEntity);
                    Assert.NotNull(GeneralEntity.HireDate);
                    Assert.NotNull(GeneralEntity.HireDate.FieldID);
                    Assert.NotEqual(Guid.Empty, GeneralEntity.HireDate.Guid);
                    Assert.False(GeneralEntity.HireDate.Dirty);
                    Assert.Equal(Strings.Hire_Date, GeneralEntity.HireDate.Type.Name);
                    Assert.Equal(Strings.Hire_Date, GeneralEntity.HireDate.Name);

                    UserTypeEntity FirstJob = NewUser.EffectiveRecord.GetFirstEntityOrDefault<UserTypeEntity>();

                    Assert.NotNull(FirstJob);
                    Assert.NotNull(FirstJob.UserType);
                    Assert.NotNull(FirstJob.UserType.FieldID);
                    Assert.NotEqual(Guid.Empty, FirstJob.UserType.Guid);
                    Assert.False(FirstJob.UserType.Dirty);
                    Assert.Equal(Strings.Job_Title, FirstJob.UserType.Type.Name);
                    Assert.Equal(Strings.Job_Title, FirstJob.UserType.Name);

                    NewUser.EffectiveDate = new DateTime(2015, 2, 2);

                    UserTypeEntity SecondJob = NewUser.EffectiveRecord.GetFirstEntityOrDefault<UserTypeEntity>();

                    Assert.NotNull(SecondJob);
                    Assert.NotNull(SecondJob.UserType);
                    Assert.NotNull(SecondJob.UserType.FieldID);
                    Assert.NotEqual(Guid.Empty, SecondJob.UserType.Guid);
                    Assert.False(SecondJob.UserType.Dirty);
                    Assert.Equal(Strings.Job_Title, SecondJob.UserType.Type.Name);
                    Assert.Equal(Strings.Job_Title, SecondJob.UserType.Name);
                }
            }

            [Fact]
            public void CreateNewEntity()
            {
                using (IKernel Kernel = new StandardKernel(new Configure()))
                {
                    Core.Models.Db.Item Item = TempItems.First();

                    User NewUser = Kernel.Get<User>(new ConstructorArgument("UserID", Item.ItemID.Value));
                    NewUser.EffectiveDate = new DateTime(2015, 1, 1);

                    var Address = NewUser.EffectiveRecord.GetOrCreateEntity<AddressEntity>();

                    Assert.NotNull(Address);
                    Assert.True(Address.Dirty);
                    Assert.False(Address.EntityID.HasValue);
                    Assert.Equal(NewUser.EffectiveRecord.EffectiveDate, Address.EffectiveDate);

                }
            }

            [Fact]
            public void SavingFields()
            {
                using (IKernel Kernel = new StandardKernel())
                {
                    Kernel.Load(new EffectFramework.Core.Configure());
                    Core.Models.Db.Item Item = TempItems.First();

                    User NewUser = Kernel.Get<User>(new ConstructorArgument("UserID", Item.ItemID.Value));
                    NewUser.EffectiveDate = new DateTime(2015, 1, 1);

                    FieldString JobTitle = NewUser.EffectiveRecord.GetFirstEntityOrDefault<UserTypeEntity>().UserType;

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

                    JobTitle.Value = "Subscriber";

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

                    User NewUser = Kernel.Get<User>(new ConstructorArgument("UserID", Item.ItemID.Value));
                    NewUser.EffectiveDate = new DateTime(2015, 1, 1);

                    UserTypeEntity Job = NewUser.EffectiveRecord.GetFirstEntityOrDefault<UserTypeEntity>();
                    Job.JobStartDate.Value = new DateTime(2015, 1, 1);

                    Job.PersistToDatabase();

                    using (var db = new ItemDb7Context(Configuration["Data:DefaultConnection:ConnectionString"]))
                    {
                        TempField.Add(db.Fields.Where(f => f.FieldID == Job.JobStartDate.FieldID.Value).Single());
                    }

                    NewUser = Kernel.Get<User>(new ConstructorArgument("UserID", Item.ItemID.Value));
                    NewUser.EffectiveDate = new DateTime(2015, 1, 1);

                    Job = NewUser.EffectiveRecord.GetFirstEntityOrDefault<UserTypeEntity>();
                    Assert.Equal(new DateTime(2015, 1, 1), Job.JobStartDate.Value);

                }
            }

            [Fact]
            public void PopulateForm()
            {
                User User = null;
                using (IKernel Kernel = new StandardKernel(new Configure()))
                {
                    User = Kernel.Get<User>(new ConstructorArgument("UserID", TempItems.First().ItemID));
                }

                User.EffectiveDate = new DateTime(2015, 3, 1);
                GeneralInfoEntity Entity = User.EffectiveRecord.CreateEntityAndAdjustNeighbors<GeneralInfoEntity>(CopyValuesFromPrevious: true);
                Entity.First_Name.Value = "Bobby";
                User.PersistToDatabase();

                using (IKernel Kernel = new StandardKernel(new Configure()))
                {
                    User = Kernel.Get<User>(new ConstructorArgument("UserID", TempItems.First().ItemID));
                }
                User.EffectiveDate = new DateTime(2015, 1, 1);
                GeneralInfoForm Form = new GeneralInfoForm()
                {
                    GeneralInfoID = User.EffectiveRecord.GetFirstEntityOrDefault<GeneralInfoEntity>().EntityID,
                };
                Form.BindTo(User);
                Form.PopulateForm();

                Entity = User.EffectiveRecord.GetFirstEntityOrDefault<GeneralInfoEntity>();

                Assert.NotNull(Entity);
                Assert.Equal(Entity.First_Name.Value, Form.First_Name);
                Assert.Equal(Entity.Last_Name.Value, Form.Last_Name);
                Assert.Equal(Entity.EntityID.Value, Form.GeneralInfoID);

                using (IKernel Kernel = new StandardKernel(new Configure()))
                {
                    User = Kernel.Get<User>(new ConstructorArgument("UserID", TempItems.First().ItemID));
                }
                User.EffectiveDate = new DateTime(2015, 1, 1);
                Form = new GeneralInfoForm();
                Form.BindTo(User);
                Form.PopulateForm();

                Entity = User.EffectiveRecord.GetFirstEntityOrDefault<GeneralInfoEntity>();

                Assert.NotNull(Entity);
                Assert.Equal(Entity.First_Name.Value, Form.First_Name);
                Assert.Equal(Entity.Last_Name.Value, Form.Last_Name);
                Assert.Equal(Entity.EntityID.Value, Form.GeneralInfoID);

                using (IKernel Kernel = new StandardKernel(new Configure()))
                {
                    User = Kernel.Get<User>(new ConstructorArgument("UserID", TempItems.First().ItemID));
                }
                User.EffectiveDate = new DateTime(2015, 3, 1);
                Form = new GeneralInfoForm()
                {
                    GeneralInfoID = User.EffectiveRecord.GetFirstEntityOrDefault<GeneralInfoEntity>().EntityID,
                    Effective_Date = new DateTime(2015, 3, 1),
                };
                Form.BindTo(User);
                Form.PopulateForm();

                Entity = User.EffectiveRecord.GetFirstEntityOrDefault<GeneralInfoEntity>();

                Assert.NotNull(Entity);
                Assert.Equal(Entity.First_Name.Value, Form.First_Name);
                Assert.Equal(Entity.Last_Name.Value, Form.Last_Name);
                Assert.Equal(Entity.EntityID.Value, Form.GeneralInfoID);
                Assert.Equal("Bobby", Form.First_Name);
            }

            [Fact]
            public void PersistForm()
            {
                User User = null;

                using (IKernel Kernel = new StandardKernel(new Configure()))
                {
                    User = Kernel.Get<User>(new ConstructorArgument("UserID", TempItems.First().ItemID));
                }
                User.EffectiveDate = new DateTime(2015, 1, 1);
                GeneralInfoForm Form = new GeneralInfoForm();
                Form.BindTo(User);
                Form.PopulateForm();

                Form.Effective_Date = new DateTime(2015, 4, 1);
                Form.GeneralInfoID = null;
                Form.First_Name = "Johnny";
                Form.Last_Name = "Jones";
                Form.PushValuesToModel();

                User.PersistToDatabase();

                using (IKernel Kernel = new StandardKernel(new Configure()))
                {
                    User = Kernel.Get<User>(new ConstructorArgument("UserID", TempItems.First().ItemID));
                }
                User.EffectiveDate = new DateTime(2015, 4, 1);
                var Entity = User.EffectiveRecord.GetFirstEntityOrDefault<GeneralInfoEntity>();

                Assert.NotNull(Entity);
                Assert.Equal(Form.First_Name, Entity.First_Name.Value);
                Assert.Equal(Form.Last_Name, Entity.Last_Name.Value);

                User.EffectiveDate = new DateTime(2015, 1, 1);

                Entity = User.EffectiveRecord.GetFirstEntityOrDefault<GeneralInfoEntity>();
                Assert.NotNull(Entity);
                Assert.Equal("John", Entity.First_Name.Value);
                Assert.Equal("Smith", Entity.Last_Name.Value);

            }

            [Fact]
            public void EnsureConcurrentUpdatesDontOverwriteEachOther()
            {
                using (IKernel Kernel = new StandardKernel(new Configure()))
                {
                    User User1 = Kernel.Get<User>(new ConstructorArgument("UserID", TempItems.First().ItemID));
                    User User2 = Kernel.Get<User>(new ConstructorArgument("UserID", TempItems.First().ItemID));

                    User1.EffectiveRecord.GetFirstEntityOrDefault<UserTypeEntity>().UserType.Value = "Administrator";
                    User2.EffectiveRecord.GetFirstEntityOrDefault<UserTypeEntity>().UserType.Value = "Pion";

                    User1.PersistToDatabase();
                    Assert.Throws(typeof(GuidMismatchException), () => { User2.PersistToDatabase(); });

                    User2.Load();
                    Assert.Equal("Administrator", User2.EffectiveRecord.GetFirstEntityOrDefault<UserTypeEntity>().UserType.Value);
                }
            }

            [Fact]
            public void EnsureButtSplicedEntitiesDontPersist()
            {
                using (IKernel Kernel = new StandardKernel(new Configure()))
                {
                    User User = Kernel.Get<User>(new ConstructorArgument("UserID", TempItems.First().ItemID));

                    User.EffectiveRecord.CreateEntityAndAdjustNeighbors<GeneralInfoEntity>(true);

                    User.PersistToDatabase();

                    Assert.Equal(1, User.AllEntities.Where(e => e.Type == TestEntityType.General_Info).Count());
                    User.Load();

                    Assert.Equal(1, User.AllEntities.Where(e => e.Type == TestEntityType.General_Info).Count());

                    User.EffectiveRecord.CreateEntityAndAdjustNeighbors<GeneralInfoEntity>(true, new DateTime(2015, 9, 1));
                    User.PersistToDatabase();

                    Assert.Equal(1, User.AllEntities.Where(e => e.Type == TestEntityType.General_Info).Count());
                    User.Load();

                    Assert.Equal(1, User.AllEntities.Where(e => e.Type == TestEntityType.General_Info).Count());
                    Assert.Equal(new DateTime(2015, 9, 1), User.AllEntities.Where(e => e.Type == TestEntityType.General_Info).First().EndEffectiveDate);
                }
            }

            [Fact]
            public void EnsurePushingToModelWithNoEffectiveDateUsesNow()
            {
                using (IKernel Kernel = new StandardKernel(new Configure()))
                {
                    User User = Kernel.Get<User>(new ConstructorArgument("UserID", TempItems.First().ItemID));

                    DateTime Today = DateTime.Now.Date;

                    GeneralInfoForm GeneralInfo = new GeneralInfoForm();
                    GeneralInfo.BindTo(User);
                    GeneralInfo.First_Name = "Johann";
                    GeneralInfo.PushValuesToModel();

                    User.PersistToDatabase();

                    User.Load();
                    User.EffectiveDate = DateTime.Now.Date;

                    GeneralInfoEntity Entity = User.EffectiveRecord.GetFirstEntityOrDefault<GeneralInfoEntity>();
                    Assert.Equal("Johann", Entity.First_Name.Value);
                    Assert.Equal(Today, Entity.EffectiveDate.Date);

                    User.EffectiveDate = new DateTime(2015, 1, 1);
                    Entity = User.EffectiveRecord.GetFirstEntityOrDefault<GeneralInfoEntity>();

                    Assert.Equal("John", Entity.First_Name.Value);
                }
            }

            public void Dispose()
            {
                TearDownEF7Database();
            }
        }

        #region Entities, Fields And Forms

        [Bind(typeof(User), typeof(GeneralInfoEntity), "GeneralInfoID")]
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
            public static readonly TestEntityType User_Role = new TestEntityType(Strings.Job, 1, typeof(UserTypeEntity));
            public static readonly TestEntityType Address = new TestEntityType(Strings.Address, 2, typeof(AddressEntity));
            public static readonly TestEntityType General_Info = new TestEntityType(Strings.General_Info, 3, typeof(GeneralInfoEntity));

            protected TestEntityType(string Name, int Value, Type Type) : base(Name, Value, Type) { }
        }

        public class TestFieldType : FieldType
        {
            protected TestFieldType(string Name, int Value, DataType DataType) : base(Name, Value, DataType) { }

            public static readonly TestFieldType User_Type = new TestFieldType(Strings.Job_Title, 1, DataType.Text);
            public static readonly TestFieldType Job_Start_Date = new TestFieldType(Strings.Job_Start_Date, 2, DataType.Date);
            public static readonly TestFieldType User_Start_Date = new TestFieldType(Strings.Hire_Date, 3, DataType.Date);
            public static readonly TestFieldType First_Name = new TestFieldType(Strings.First_Name, 4, DataType.Text);
            public static readonly TestFieldType Last_Name = new TestFieldType(Strings.Last_Name, 5, DataType.Text);

        }

        public class TestItemType : ItemType
        {
            protected TestItemType(string Name, int Value, Type Type) : base(Name, Value, Type) { }

            public static readonly TestItemType User = new TestItemType("User", 1, typeof(User));

        }

        public class User : Core.Models.Item
        {
            public override ItemType Type
            {
                get
                {
                    return TestItemType.User;
                }
            }

            public User(IPersistenceService PersistenceService) : base(PersistenceService) { }

            public User(int UserID, IPersistenceService PersistenceService, bool LoadItem = true) : base(UserID, PersistenceService, LoadItem) { }
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
                HireDate = new FieldDate(TestFieldType.User_Start_Date, PersistenceService);
                First_Name = new FieldString(TestFieldType.First_Name, PersistenceService);
                Last_Name = new FieldString(TestFieldType.Last_Name, PersistenceService);
            }

            public FieldDate HireDate { get; private set; }
            public FieldString First_Name { get; private set; }
            public FieldString Last_Name { get; private set; }
        }

        public class UserTypeEntity : EntityBase
        {

            public override EntityType Type
            {
                get
                {
                    return TestEntityType.User_Role;
                }
            }

            public UserTypeEntity() : base() { }

            public UserTypeEntity(IPersistenceService PersistenceService)
                : base(PersistenceService)
            {

            }

            protected override void WireUpFields()
            {
                UserType = new FieldString(TestFieldType.User_Type, PersistenceService);
                JobStartDate = new FieldDate(TestFieldType.Job_Start_Date, PersistenceService);
            }

            public FieldString UserType { get; private set; }
            public FieldDate JobStartDate { get; private set; }
        }

        #endregion
    }
}