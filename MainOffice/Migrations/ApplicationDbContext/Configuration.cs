namespace MainOffice.Migrations.ApplicationDbContext
{
    using MainOffice.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MainOffice.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Migrations\ApplicationDbContext";
        }

        protected override void Seed(MainOffice.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            var store = new RoleStore<ApplicationRole>(context);
            var manager = new RoleManager<ApplicationRole>(store);
            string[] roles = { "visitor", "admin", "director", "secretary", "salonadmin", "worksheets", "pin", "owner" };
            foreach (string roleName in roles)
            {
                if (!context.Roles.Any(r => r.Name == roleName))
                {
                    manager.Create(new ApplicationRole { Name = roleName });
                }
            }

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            var user = new ApplicationUser { UserName = "salon.londa@gmail.com", Email = "salon.londa@gmail.com", EmailConfirmed = true, Locality = "ru-RU" };
            if (!context.Users.Any(u => u.UserName == "salon.londa@gmail.com"))
            {
                userManager.Create(user, "DWnKp5z^");
                userManager.AddToRole(user.Id, "visitor");
                userManager.AddToRole(user.Id, "admin");
            }
            if (!context.Users.Any(u => u.Email == "salon.londa.pechersk@gmail.com"))
            {
                user = new ApplicationUser { UserName = "salon.londa.pechersk@gmail.com", Email = "salon.londa.pechersk@gmail.com", EmailConfirmed = true, Locality = "uk-UA" };

                userManager.Create(user, "lonD@5404");
                userManager.AddToRole(user.Id, "worksheets");
                userManager.AddToRole(user.Id, "pin");
            }
            if (!context.Users.Any(u => u.Email == "salon.londa.darnica@gmail.com"))
            {
                user = new ApplicationUser { UserName = "salon.londa.darnica@gmail.com", Email = "salon.londa.darnica@gmail.com", EmailConfirmed = true, Locality = "uk-UA" };

                userManager.Create(user, "lonD@1810");
                userManager.AddToRole(user.Id, "worksheets");
                userManager.AddToRole(user.Id, "pin");
            }
            if (!context.Users.Any(u => u.Email == "salon.londa.kpi@gmail.com"))
            {
                user = new ApplicationUser { UserName = "salon.londa.kpi@gmail.com", Email = "salon.londa.kpi@gmail.com", EmailConfirmed = true, Locality = "uk-UA" };

                userManager.Create(user, "lonD@1806");
                userManager.AddToRole(user.Id, "worksheets");
                userManager.AddToRole(user.Id, "pin");
            }
            if (!context.Users.Any(u => u.Email == "salon.londa.pestelya@gmail.com"))
            {
                user = new ApplicationUser { UserName = "salon.londa.pestelya@gmail.com", Email = "salon.londa.pestelya@gmail.com", EmailConfirmed = true, Locality = "uk-UA" };

                userManager.Create(user, "lonD@1807");
                userManager.AddToRole(user.Id, "worksheets");
                userManager.AddToRole(user.Id, "pin");
            }
            if (!context.Users.Any(u => u.Email == "salon.londa.minskaja@gmail.com"))
            {
                user = new ApplicationUser { UserName = "salon.londa.minskaja@gmail.com", Email = "salon.londa.minskaja@gmail.com", EmailConfirmed = true, Locality = "uk-UA" };

                userManager.Create(user, "lonD@6111");
                userManager.AddToRole(user.Id, "worksheets");
                userManager.AddToRole(user.Id, "pin");
            }
            if (!context.Users.Any(u => u.Email == "salon.londa.radujni@gmail.com"))
            {
                user = new ApplicationUser { UserName = "salon.londa.radujni@gmail.com", Email = "salon.londa.radujni@gmail.com", EmailConfirmed = true, Locality = "uk-UA" };

                userManager.Create(user, "lonD@1809");
                userManager.AddToRole(user.Id, "worksheets");
                userManager.AddToRole(user.Id, "pin");
            }


            context.Localities.AddOrUpdate(p => p.Name,
                new Locality { Name = "uk-UA", Language = "Українська" },
                new Locality { Name = "ru-RU", Language = "Русский" });

            context.SaveChanges();
        }
    }
}
