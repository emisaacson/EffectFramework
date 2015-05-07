using Microsoft.Data.Entity;

namespace EffectFramework.Core.Models.Db
{
    public class HrmsDb7Context : DbContext, IDbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemRecord> ItemRecords { get; set; }
        public DbSet<Entity> Entities { get; set; }
        public DbSet<EntityField> Fields { get; set; }
        public DbSet<ItemEntity> ItemEntities { get; set; }

        public void usp_DeleteEntireDatabase(bool ForReal = false)
        {
            if (ForReal)
            {
                var Command = this.Database.AsRelational().Connection.DbConnection.CreateCommand();
                Command.CommandText = "EXEC dbo.usp_DeleteEntireDatabase;";
                Command.ExecuteNonQuery();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Visual Studio 2015 | Use the LocalDb 12 instance created by Visual Studio
            optionsBuilder.UseSqlServer(@"Server=EMIKEKDC02;Database=HRMS;Trusted_Connection=True;MultipleActiveResultSets=True;");

            // Visual Studio 2013 | Use the LocalDb 11 instance created by Visual Studio
            //optionsBuilder.UseSqlServer(@"Server=(localdb)\v11.0;Database=Blogging;Trusted_Connection=True;");

            // Visual Studio 2012 | Use the SQL Express instance created by Visual Studio
            //optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=Blogging;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ForSqlServer().UseIdentity();

            builder.Entity<ItemRecord>(e =>
            {
                e.ForRelational(rb =>
                {
                    rb.Table("ItemRecords");
                });

                e.Key(er => er.ItemRecordID);

                e.Property(er => er.ItemRecordID).ForSqlServer().UseIdentity();

            });

            builder.Entity<Item>(e =>
            {
                e.ForRelational(rb =>
                {
                    rb.Table("Items");
                });

                e.Key(em => em.ItemID);

                e.Property(em => em.ItemID).ForSqlServer().UseIdentity();

                e.Collection(er => er.ItemRecords)
                    .InverseReference(em => em.Item)
                    .ForeignKey(em => em.ItemID);
            });


            builder.Entity<Entity>(e =>
            {
                e.ForRelational(rb =>
                {
                    rb.Table("Entities");
                });

                e.Key(en => en.EntityID);

                e.Property(en => en.EntityID).ForSqlServer().UseIdentity();

                e.Reference(em => em.Item)
                    .InverseReference()
                    .ForeignKey<Entity>(en => en.ItemID);

                e.Collection(en => en.EntityFields)
                    .InverseReference(ef => ef.Entity)
                    .ForeignKey(ef => ef.EntityID);
            });



            builder.Entity<EntityField>(e => {
                e.ForRelational(rb =>
                {
                    rb.Table("EntityFields");
                });

                e.Key(ef => ef.EntityFieldID);

                e.Property(ef => ef.EntityFieldID).ForSqlServer().UseIdentity();
            });
                

            builder.Entity<ItemEntity>(e => {
                e.ForRelational(rb =>
                {
                    rb.Table("ItemEntities");
                });

                e.Key(ee => ee.ItemEntityID);

                e.Property(ee => ee.ItemEntityID).ForSqlServer().UseIdentity();

                e.Index(ee => new { ee.ItemRecordID, ee.ItemEntityID });
            });
                
        }
    }
}
