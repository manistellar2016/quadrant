using MyNPO.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace MyNPO.DataAccess
{
    public class EntityContext : DbContext
    {
        public EntityContext(string connectionString) : base(connectionString) { }
        public DbSet<LoginInfo> loginInfos { get; set; }
        public DbSet<FamilyInfo> familyInfos { get; set; }
        public DbSet<DependentInfo> dependentInfos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            Database.SetInitializer<EntityContext>(new DropCreateDatabaseIfModelChanges<EntityContext>());
            //modelBuilder.Entity<LoginInfo>().Property(q => q.UserId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); // Both ([DatabaseGenerated(DatabaseGeneratedOption.Identity)]) is doing the same behaviour
        }


    }
}