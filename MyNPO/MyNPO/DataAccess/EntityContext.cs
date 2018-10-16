using MyNPO.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace MyNPO.DataAccess
{
    public class EntityContext : DbContext
    {
        public static string connectionString = ConfigurationManager.AppSettings["DbConnectionString"];
        public EntityContext(): base(connectionString)
        {
           

        }
        public DbSet<LoginInfo> loginInfos { get; set; }
        public DbSet<FamilyInfo> familyInfos { get; set; }
        public DbSet<DependentInfo> dependentInfos { get; set; }
        public DbSet<Report> reportInfo { get; set; }
        public DbSet<Event> eventInfos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<EntityContext>(new MigrateDatabaseToLatestVersion<EntityContext, MyConfiguration>());

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;           

            base.OnModelCreating(modelBuilder);
       
        }


    }

    public class MyConfiguration : System.Data.Entity.Migrations.DbMigrationsConfiguration<EntityContext>
    {       
        public MyConfiguration()
        {            
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
            ContextKey = "MyNPO.DataAccess.EntityContext";
        }
    }

  

}