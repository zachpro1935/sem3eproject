namespace eproject.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using eproject.Models;
    using eproject.Security;
    internal sealed class Configuration : DbMigrationsConfiguration<eproject.Models.context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(eproject.Models.context context)
        {
            // add default admin account
            context.user.AddOrUpdate(x => x.email,
        new User()
        {
            name = "zach",
            role = "ROLE_ADMIN",
            enabled = true,
            pass = pass.HashPassword("admin1234"),
            email = "admin@gmail.com",
            expireDate = new DateTime(2050, 12, 30),
            gender = "male",
            phone = "0906767296"
        }
        );


        }
    }
}
