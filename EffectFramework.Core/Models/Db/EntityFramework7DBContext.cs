using System;
using Microsoft.Data.Entity;

namespace EffectFramework.Core.Models.Db
{
    public class EntityFramework7DBContext : DbContext, IDbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<Entity> Entities { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<Lookup> Lookups { get; set; }
        public DbSet<LookupType> LookupTypes { get; set; }
        public DbSet<CompleteItem> CompleteItems { get; set; }
        public DbSet<FieldTypeMeta> FieldMetaTypes { get; set; }
        private string ConnectionString { get; set; }

        public void usp_DeleteEntireDatabase(bool ForReal = false)
        {
            if (ForReal)
            {
                using (var Command = this.Database.AsRelational().Connection.DbConnection.CreateCommand())
                {
                    Command.Connection.Open();
                    Command.CommandText = "EXEC dbo.usp_DeleteEntireDatabase;";
                    Command.ExecuteNonQuery();
                }
            }
        }

        public EntityFramework7DBContext(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }

        protected override void OnConfiguring(EntityOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(this.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ForSqlServer().UseIdentity();

            builder.Entity<CompleteItem>(e =>
            {
                e.Table("CompleteItems");

                e.Key(i => new { i.ItemID, i.EntityID, i.FieldID });
            });

            builder.Entity<FieldTypeMeta>(e =>
            {
                e.Table("FieldTypeMeta");

                e.Key(i => i.FieldTypeMetaID);
            });

            builder.Entity<Item>(e =>
            {
                e.Table("Items");

                e.Key(i => i.ItemID);

                e.Property(i => i.ItemID).ForSqlServer().UseIdentity();

                e.Collection(i => i.Entities)
                    .InverseReference(en => en.Item)
                    .ForeignKey(en => en.ItemID);
            });

            builder.Entity<Lookup>(l =>
            {
                l.Table("Lookups");

                l.Key(e => e.LookupID);

                l.Property(e => e.LookupID).ForSqlServer().UseIdentity();

                l.Reference<Field>().InverseReference(e => e.Lookup).ForeignKey<Field>(e => e.ValueLookup);
            });

            builder.Entity<LookupType>(lt =>
            {
                lt.Table("LookupTypes");

                lt.Key(e => e.LookupTypeID);

                lt.Property(l => l.LookupTypeID).ForSqlServer().UseIdentity();

            });

            builder.Entity<Entity>(e =>
            {
                e.Table("Entities");

                e.Key(en => en.EntityID);

                e.Property(en => en.EntityID).ForSqlServer().UseIdentity();

                e.Collection(en => en.EntityFields)
                    .InverseReference(ef => ef.Entity)
                    .ForeignKey(ef => ef.EntityID);
            });

            builder.Entity<Field>(e => {
                e.Table("Fields");

                e.Key(ef => ef.FieldID);

                e.Property(ef => ef.FieldID).ForSqlServer().UseIdentity();
            });
                

        }

        public IDisposable BeginTransaction()
        {
            return this.Database.AsRelational().Connection.BeginTransaction();
        }

        public void Rollback()
        {
            this.Database.AsRelational().Connection.DbTransaction.Rollback();
        }

        public void Commit()
        {
            this.Database.AsRelational().Connection.DbTransaction.Commit();
        }
    }
}
