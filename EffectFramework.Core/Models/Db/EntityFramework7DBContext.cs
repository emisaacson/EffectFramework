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
        public DbSet<FieldTypeMeta> FieldTypeMetas { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<ItemType> ItemTypes { get; set; }
        public DbSet<EntityType> EntityTypes { get; set; }
        public DbSet<FieldType> FieldTypes { get; set; }

        protected string ConnectionString { get; set; }

        public void usp_DeleteEntireDatabase(bool ForReal = false)
        {
            if (ForReal)
            {
                using (var Command = this.Database.GetDbConnection().CreateCommand())
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(this.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.UseSqlServerIdentityColumns();

            builder.Entity<CompleteItem>(e =>
            {
                e.ToTable("CompleteItems");

                e.HasKey(i => new { i.ItemID, i.EntityID, i.FieldID });
            });

            builder.Entity<FieldTypeMeta>(e =>
            {
                e.ToTable("FieldTypeMeta");

                e.HasKey(i => i.FieldTypeMetaID);

                e.Property(i => i.FieldTypeID).UseSqlServerIdentityColumn();
            });

            builder.Entity<AuditLog>(e =>
            {
                e.ToTable("AuditLog");

                e.HasKey(i => i.AuditLogID);

                e.Property(i => i.AuditLogID).UseSqlServerIdentityColumn();
            });

            builder.Entity<Item>(e =>
            {
                e.ToTable("Items");

                e.HasKey(i => i.ItemID);

                e.Property(i => i.ItemID).UseSqlServerIdentityColumn();

                e.HasMany(i => i.Entities)
                    .WithOne(en => en.Item)
                    .ForeignKey(en => en.ItemID);
            });

            builder.Entity<Lookup>(l =>
            {
                l.ToTable("Lookups");

                l.HasKey(e => e.LookupID);

                l.Property(e => e.LookupID).UseSqlServerIdentityColumn();

                l.HasOne<Field>().WithOne(e => e.Lookup).ForeignKey<Field>(e => e.ValueLookup);
                l.HasOne<LookupType>().WithMany(e => e.Lookups).ForeignKey(e => e.LookupTypeID);
                l.HasOne<Lookup>().WithOne(e => e.Parent).ForeignKey<Lookup>(e => e.ParentID);
            });

            builder.Entity<LookupType>(lt =>
            {
                lt.ToTable("LookupTypes");

                lt.HasKey(e => e.LookupTypeID);

                lt.Property(l => l.LookupTypeID).UseSqlServerIdentityColumn();
                
            });

            builder.Entity<Entity>(e =>
            {
                e.ToTable("Entities");

                e.HasKey(en => en.EntityID);

                e.Property(en => en.EntityID).UseSqlServerIdentityColumn();

                e.HasMany(en => en.EntityFields)
                    .WithOne(ef => ef.Entity)
                    .ForeignKey(ef => ef.EntityID);
            });

            builder.Entity<Field>(e =>
            {
                e.ToTable("Fields");

                e.HasKey(ef => ef.FieldID);

                e.Property(ef => ef.FieldID).UseSqlServerIdentityColumn();
            });

            builder.Entity<ItemType>(e =>
            {
                e.ToTable("ItemTypes");

                e.HasKey(it => it.ItemTypeID);

                e.Property(it => it.ItemTypeID).UseSqlServerIdentityColumn();
            });

            builder.Entity<EntityType>(e =>
            {
                e.ToTable("EntityTypes");

                e.HasKey(et => et.EntityTypeID);

                e.Property(et => et.EntityTypeID).UseSqlServerIdentityColumn();
            });

            builder.Entity<FieldType>(e =>
            {
                e.ToTable("FieldTypes");

                e.HasKey(ft => ft.FieldTypeID);

                e.Property(ft => ft.FieldTypeID).UseSqlServerIdentityColumn();
            });
        }

        public IDisposable BeginTransaction()
        {
            return this.Database.BeginTransaction();
        }

        public void Rollback()
        {
            this.Database.RollbackTransaction();
        }

        public void Commit()
        {
            this.Database.CommitTransaction();
        }
    }
}
