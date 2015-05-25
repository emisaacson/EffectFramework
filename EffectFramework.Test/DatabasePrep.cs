using EffectFramework.Core.Models.Db;
using Microsoft.Data.Entity;
using Microsoft.Framework.ConfigurationModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EffectFramework.Test
{
    public partial class EffectFrameworkTests
    {
        public static string _BasePath = null;
        public static string BasePath
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

        public static List<Core.Models.Db.Item> TempItems = new List<Core.Models.Db.Item>();
        public static List<Entity> TempEntity = new List<Entity>();
        public static List<Field> TempField = new List<Field>();

        public static IConfiguration Configuration { get; set; }

        public static void PrepareEF7Database()
        {
            TempItems.Clear();
            TempEntity.Clear();
            TempField.Clear();

            using (var db = new ItemDb7Context(Configuration["Data:DefaultConnection:ConnectionString"]))
            using (db.BeginTransaction())
            {
                Core.Models.Db.Item NewItem = new Core.Models.Db.Item()
                {
                    ItemTypeID = TestItemType.User.Value,
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
                    EntityTypeID = TestEntityType.User_Role,
                    IsDeleted = false,
                    Guid = Guid.NewGuid(),
                };

                Entity NewEntity3 = new Entity()
                {
                    EffectiveDate = new DateTime(2015, 2, 1),
                    EndEffectiveDate = null,
                    ItemID = NewItem.ItemID.Value,
                    EntityTypeID = TestEntityType.User_Role,
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
                    FieldTypeID = TestFieldType.User_Start_Date.Value,
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
                    FieldTypeID = TestFieldType.User_Type.Value,
                    IsDeleted = false,
                    ValueText = "Subscriber",
                    Guid = Guid.NewGuid(),
                };

                Field NewField3 = new Field()
                {
                    EntityID = NewEntity3.EntityID,
                    FieldTypeID = TestFieldType.User_Type.Value,
                    IsDeleted = false,
                    ValueText = "Editor",
                    Guid = Guid.NewGuid(),
                };

                db.Fields.Add(NewField1);
                db.Fields.Add(NewField2);
                db.Fields.Add(NewField3);
                db.Fields.Add(NewField4);
                db.Fields.Add(NewField5);
                db.SaveChanges();

                db.Database.AsRelational().Connection.Transaction.Commit();

                TempItems.Add(NewItem);
            }
        }

        public static void TearDownEF7Database()
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
                    var OtherFields = db.Fields.Where(f => f.EntityID == Entity.EntityID);
                    foreach (var Field in OtherFields)
                    {
                        db.Fields.Remove(Field);
                    }
                    db.Entities.Remove(Entity);
                }
                db.SaveChanges();

                foreach (var Item in TempItems)
                {
                    var OtherEntities = db.Entities.Where(e => e.ItemID == Item.ItemID);
                    foreach (var Entity in OtherEntities)
                    {
                        db.Entities.Remove(Entity);
                    }
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
}
