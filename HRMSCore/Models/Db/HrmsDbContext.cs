using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace HRMS.Core.Models.Db
{
    public class HrmsDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeRecord> EmployeeRecords { get; set; }
        public DbSet<Entity> Entities { get; set; }
        public DbSet<EntityField> Fields { get; set; }
        public DbSet<EmployeeEntity> EmployeeEntities { get; set; }

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

            builder.Entity<EmployeeRecord>(e =>
            {
                e.ForRelational(rb =>
                {
                    rb.Table("EmployeeRecords");
                });

                e.Key(er => er.EmployeeRecordID);

                e.Property(er => er.EmployeeRecordID).ForSqlServer().UseIdentity();

            });

            builder.Entity<Employee>(e =>
            {
                e.ForRelational(rb =>
                {
                    rb.Table("Employees");
                });

                e.Key(em => em.EmployeeID);

                e.Property(em => em.EmployeeID).ForSqlServer().UseIdentity();

                e.Collection(er => er.EmployeeRecords)
                    .InverseReference(em => em.Employee)
                    .ForeignKey(em => em.EmployeeID);
            });


            builder.Entity<Entity>(e =>
            {
                e.ForRelational(rb =>
                {
                    rb.Table("Entities");
                });

                e.Key(en => en.EntityID);

                e.Property(en => en.EntityID).ForSqlServer().UseIdentity();

                e.Reference(em => em.Employee)
                    .InverseReference()
                    .ForeignKey<Entity>(en => en.EmployeeID);

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
                

            builder.Entity<EmployeeEntity>(e => {
                e.ForRelational(rb =>
                {
                    rb.Table("EmployeeEntities");
                });

                e.Key(ee => ee.EmployeeEntityID);

                e.Property(ee => ee.EmployeeEntityID).ForSqlServer().UseIdentity();

                e.Index(ee => new { ee.EmployeeRecordID, ee.EmployeeEntityID });
            });
                
        }
    }
}
