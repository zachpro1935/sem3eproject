using eproject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eproject.Models
{
    public class context : DbContext
    {

        public context() : base("db") { }
        public DbSet<User> user { get; set; }

        public DbSet<Recipe> recipe { get; set; }
        public DbSet<FeedBack> feedBack { get; set; }
        public DbSet<Contest> contest { get; set; }
        public DbSet<Ratting> ratting { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<context>(null);
            base.OnModelCreating(modelBuilder);
        }
    }
}