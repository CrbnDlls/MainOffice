namespace MainOffice.Migrations.AppDbContext
{
    using MainOffice.Functions;
    using MainOffice.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MainOffice.Models.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Migrations\AppDbContext";
        }

        protected override void Seed(MainOffice.Models.AppDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            context.BarberLevels.AddOrUpdate(p => p.Name,
                new BarberLevel { Name = "VIP Майстер", OrderNumber = 4, Color = "E87701" },
                new BarberLevel { Name = "Високої Кваліфікації", OrderNumber = 8, Color = "7F00BA" },
                new BarberLevel { Name = "Авторський Майстер", OrderNumber = 6, Color = "146400" },
                new BarberLevel { Name = "Гранд Майстер", OrderNumber = 3, Color = "E23C02" },
                new BarberLevel { Name = "Стиль Директор", OrderNumber = 1, Color = "20FF00" },
                new BarberLevel { Name = "Майстер Року", OrderNumber = 7, Color = "00CBC9" },
                new BarberLevel { Name = "Топ Майстер", OrderNumber = 2, Color = "9B2800" },
                new BarberLevel { Name = "Стиль Майстер", OrderNumber = 5, Color = "E8E701" });

            context.Companies.AddOrUpdate(p => p.Name,
                new Company { Name = "Крокус", Address = "м. Київ, Харківське шосе, 53" },
                new Company { Name = "Крокус І", Address = "м. Київ, Попудренко, 7Ж" },
                new Company { Name = "Крокус ІІ", Address = "м. Київ, Політехнічний пров., 1/33" },
                new Company { Name = "Аспазія", Address = "м. Київ, Маршала Тимошенко, 19" });

            context.Professions.AddOrUpdate(p => p.Name,
                new Profession { Name = "Адміністратор салона", OrderNumber = 6, Color = "FF7931" }, //9 - 1
                new Profession { Name = "Перукар", OrderNumber = 1, Color = "6DFF31" }, //2
                new Profession { Name = "Майстер манікюра", OrderNumber = 2, Color = "FC31FF" }, //3
                new Profession { Name = "Косметич. послуг", OrderNumber = 3, Color = "31FF90" }, //4
                new Profession { Name = "Майстер епіляції", OrderNumber = 4, Color = "FFD631" }, //5
                new Profession { Name = "Масажист", OrderNumber = 5, Color = "3231FF" }); //10 - 6

            context.SalonStates.AddOrUpdate(p => p.Name,
                new SalonState { Name = "Діючий" },
                new SalonState { Name = "Зачинен назавжди" });

            context.SalonTypes.AddOrUpdate(p => p.Name,
                new SalonType { Name = "Салон краси" },
                new SalonType { Name = "Перукарня" });

            context.SaveChanges();
            
            context.Salons.AddOrUpdate(p => p.Name,
                new Salon { Name = "метро Печерська", Address = "Київ, бул. Лесі Ураїнки, 21Б", PhoneNumber1 = "(093) 474-54-04", PhoneNumber2 = "(044) 285-05-15", SalonStateId = 1, SalonTypeId = 2, Latitude = 50.425100, Longitude = 30.543337, IP = "195.177.73.220" }, //11 - 1
                new Salon { Name = "метро Мінська", Address = "Київ, вул. Тимошенко, 19", PhoneNumber1 = "(063) 445-61-11", PhoneNumber2 = "(044) 502-36-33", SalonStateId = 1, SalonTypeId = 2, Latitude = 50.513797, Longitude = 30.496843, IP = "82.193.119.150" }, //2
                new Salon { Name = "метро Політехнічний інститут", Address = "Київ, проспект Перемоги, 33", PhoneNumber1 = "(063) 619-18-06", PhoneNumber2 = "(044) 489-16-03", SalonStateId = 1, SalonTypeId = 1, Latitude = 50.450526, Longitude = 30.467049, IP = "134.249.184.209" }, //3
                new Salon { Name = "метро Вокзальна", Address = "Київ, вул. Пестеля, 11", PhoneNumber1 = "(063) 619-18-07", PhoneNumber2 = "(044) 236-82-26", SalonStateId = 1, SalonTypeId = 1, Latitude = 50.444859, Longitude = 30.489859, IP = "195.60.229.67" }, //4
                new Salon { Name = "масив Райдужний", Address = "Київ, вул. Райдужна, 15", PhoneNumber1 = "(063) 619-18-09", PhoneNumber2 = "(044) 542-24-37", SalonStateId = 1, SalonTypeId = 1, Latitude = 50.486521, Longitude = 30.585064, IP = "94.45.65.55" }, //10 - 5
                new Salon { Name = "метро Дарниця", Address = "Київ, вул. Попудренко, 7Ж", PhoneNumber1 = "(063) 619-18-10", PhoneNumber2 = "(044) 296-82-94", SalonStateId = 1, SalonTypeId = 1, Latitude = 50.455733, Longitude = 30.613638, IP = "195.49.151.152" }); //9 - 6

            context.Trademarks.AddOrUpdate(p => p.Name,
                new Trademark { Name = "BES" },
                new Trademark { Name = "Barba Italiana" },
                new Trademark { Name = "Estel" },
                new Trademark { Name = "Londa" });

            context.Promotions.AddOrUpdate(p => p.Name,
                new Promotion { Name = "10%" },
                new Promotion { Name = "50%" },
                new Promotion { Name = "Сотрудник" });

            /*Sql("INSERT INTO dbo.PStatus (Name,Symbol,IsHidden) VALUES ('Каса','THK',0)");
            Sql("INSERT INTO dbo.PStatus (Name,Symbol,IsHidden) VALUES ('Термінал','THT',0)");
            Sql("INSERT INTO dbo.PStatus (Name,Symbol,IsHidden) VALUES ('Депозит','THД',1)");*/

            context.PStatuses.AddOrUpdate(p => p.Name,
                new PStatus { Name = "Каса", Symbol = "THK", IsHidden = false },
                new PStatus { Name = "Термінал", Symbol = "THT", IsHidden = false },
                new PStatus { Name = "Депозит", Symbol = "THД", IsHidden = true });

            context.SaveChanges();
        }
    }
}
