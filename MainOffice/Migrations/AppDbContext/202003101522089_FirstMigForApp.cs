namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstMigForApp : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BarberLevels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "IX_NameUnique");
            
            CreateTable(
                "dbo.CashRegCodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.Int(nullable: false),
                        PriceListUnitId = c.Int(),
                        ProductId = c.Int(),
                        ServiceId = c.Int(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price10 = c.Decimal(precision: 18, scale: 2),
                        Price50 = c.Decimal(precision: 18, scale: 2),
                        PriceStaff = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PriceListUnits", t => t.PriceListUnitId)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .ForeignKey("dbo.Services", t => t.ServiceId)
                .Index(t => t.Code, unique: true, name: "IX_CodeUnique")
                .Index(t => t.PriceListUnitId)
                .Index(t => t.ProductId)
                .Index(t => t.ServiceId);
            
            CreateTable(
                "dbo.PriceListUnits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "IX_NameUnique");
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        TrademarkId = c.Int(),
                        ProductlineId = c.Int(),
                        ProductVolumeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Productlines", t => t.ProductlineId)
                .ForeignKey("dbo.ProductVolumes", t => t.ProductVolumeId, cascadeDelete: true)
                .ForeignKey("dbo.Trademarks", t => t.TrademarkId)
                .Index(t => new { t.Name, t.TrademarkId, t.ProductlineId, t.ProductVolumeId }, unique: true, name: "IX_ProductUnique");
            
            CreateTable(
                "dbo.Productlines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "IX_NameUnique");
            
            CreateTable(
                "dbo.ProductVolumes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "IX_NameUnique");
            
            CreateTable(
                "dbo.Trademarks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "IX_NameUnique");
            
            CreateTable(
                "dbo.Services",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        ServiceVolumeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ServiceVolumes", t => t.ServiceVolumeId, cascadeDelete: true)
                .Index(t => new { t.Name, t.ServiceVolumeId }, unique: true, name: "IX_ServiceUnique");
            
            CreateTable(
                "dbo.ServiceVolumes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "IX_NameUnique");
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Address = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "IX_NameUnique");
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FamilyName = c.String(nullable: false, maxLength: 50),
                        Name = c.String(nullable: false, maxLength: 50),
                        FathersName = c.String(nullable: false, maxLength: 50),
                        BirthDay = c.DateTime(nullable: false, storeType: "date"),
                        OldFamilyName = c.String(maxLength: 50),
                        ProfessionId = c.Int(),
                        BarberLevelId = c.Int(),
                        SalonId = c.Int(),
                        HireDate = c.DateTime(storeType: "date"),
                        StaffNumber = c.Int(),
                        PhoneNumber1 = c.String(maxLength: 15),
                        PhoneNumber2 = c.String(maxLength: 15),
                        CompanyId = c.Int(),
                        RegisterDate = c.DateTime(storeType: "date"),
                        DismissalDate = c.DateTime(storeType: "date"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BarberLevels", t => t.BarberLevelId)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .ForeignKey("dbo.Professions", t => t.ProfessionId)
                .ForeignKey("dbo.Salons", t => t.SalonId)
                .Index(t => new { t.FamilyName, t.Name, t.FathersName, t.BirthDay }, unique: true, name: "IX_EmployeeUnique")
                .Index(t => t.ProfessionId)
                .Index(t => t.BarberLevelId)
                .Index(t => t.SalonId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.Professions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "IX_NameUnique");
            
            CreateTable(
                "dbo.Salons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Address = c.String(nullable: false, maxLength: 100),
                        PhoneNumber1 = c.String(nullable: false, maxLength: 15),
                        PhoneNumber2 = c.String(maxLength: 15),
                        SalonStateId = c.Int(nullable: false),
                        SalonTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SalonStates", t => t.SalonStateId, cascadeDelete: true)
                .ForeignKey("dbo.SalonTypes", t => t.SalonTypeId, cascadeDelete: true)
                .Index(t => t.Name, unique: true, name: "IX_NameUnique")
                .Index(t => t.PhoneNumber1, unique: true, name: "IX_Phone1Unique")
                .Index(t => t.PhoneNumber2, unique: true, name: "IX_Phone2Unique")
                .Index(t => t.SalonStateId)
                .Index(t => t.SalonTypeId);
            
            CreateTable(
                "dbo.SalonStates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "IX_NameUnique");
            
            CreateTable(
                "dbo.SalonTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "IX_NameUnique");
            
            CreateTable(
                "dbo.DelayedUpdateEmployees",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FamilyName = c.String(nullable: false, maxLength: 50),
                        Name = c.String(nullable: false, maxLength: 50),
                        FathersName = c.String(nullable: false, maxLength: 50),
                        BirthDay = c.DateTime(nullable: false, storeType: "date"),
                        OldFamilyName = c.String(maxLength: 50),
                        ProfessionId = c.Int(),
                        BarberLevelId = c.Int(),
                        SalonId = c.Int(),
                        HireDate = c.DateTime(storeType: "date"),
                        StaffNumber = c.Int(),
                        PhoneNumber1 = c.String(maxLength: 15),
                        PhoneNumber2 = c.String(maxLength: 15),
                        CompanyId = c.Int(),
                        RegisterDate = c.DateTime(storeType: "date"),
                        DismissalDate = c.DateTime(storeType: "date"),
                        EmployeeId = c.Int(),
                        UpdateDate = c.DateTime(storeType: "date"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BarberLevels", t => t.BarberLevelId)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.Professions", t => t.ProfessionId)
                .ForeignKey("dbo.Salons", t => t.SalonId)
                .Index(t => new { t.FamilyName, t.Name, t.FathersName, t.BirthDay }, unique: true, name: "IX_EmployeeUnique")
                .Index(t => t.ProfessionId)
                .Index(t => t.BarberLevelId)
                .Index(t => t.SalonId)
                .Index(t => t.CompanyId)
                .Index(t => t.EmployeeId);

            Sql("ALTER TABLE dbo.CashRegCodes ADD CONSTRAINT CHECK_ProdOrServ CHECK ((ProductId IS NULL AND ServiceId IS NOT NULL) OR (ProductId IS NOT NULL AND ServiceId IS NULL))");
        }
        
        public override void Down()
        {
            Sql("ALTER TABLE dbo.CashRegCodes DROP CHECK CHECK_ProdOrServ");
            DropForeignKey("dbo.DelayedUpdateEmployees", "SalonId", "dbo.Salons");
            DropForeignKey("dbo.DelayedUpdateEmployees", "ProfessionId", "dbo.Professions");
            DropForeignKey("dbo.DelayedUpdateEmployees", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.DelayedUpdateEmployees", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.DelayedUpdateEmployees", "BarberLevelId", "dbo.BarberLevels");
            DropForeignKey("dbo.Employees", "SalonId", "dbo.Salons");
            DropForeignKey("dbo.Salons", "SalonTypeId", "dbo.SalonTypes");
            DropForeignKey("dbo.Salons", "SalonStateId", "dbo.SalonStates");
            DropForeignKey("dbo.Employees", "ProfessionId", "dbo.Professions");
            DropForeignKey("dbo.Employees", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Employees", "BarberLevelId", "dbo.BarberLevels");
            DropForeignKey("dbo.CashRegCodes", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.Services", "ServiceVolumeId", "dbo.ServiceVolumes");
            DropForeignKey("dbo.CashRegCodes", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "TrademarkId", "dbo.Trademarks");
            DropForeignKey("dbo.Products", "ProductVolumeId", "dbo.ProductVolumes");
            DropForeignKey("dbo.Products", "ProductlineId", "dbo.Productlines");
            DropForeignKey("dbo.CashRegCodes", "PriceListUnitId", "dbo.PriceListUnits");
            DropIndex("dbo.DelayedUpdateEmployees", new[] { "EmployeeId" });
            DropIndex("dbo.DelayedUpdateEmployees", new[] { "CompanyId" });
            DropIndex("dbo.DelayedUpdateEmployees", new[] { "SalonId" });
            DropIndex("dbo.DelayedUpdateEmployees", new[] { "BarberLevelId" });
            DropIndex("dbo.DelayedUpdateEmployees", new[] { "ProfessionId" });
            DropIndex("dbo.DelayedUpdateEmployees", "IX_EmployeeUnique");
            DropIndex("dbo.SalonTypes", "IX_NameUnique");
            DropIndex("dbo.SalonStates", "IX_NameUnique");
            DropIndex("dbo.Salons", new[] { "SalonTypeId" });
            DropIndex("dbo.Salons", new[] { "SalonStateId" });
            DropIndex("dbo.Salons", "IX_Phone2Unique");
            DropIndex("dbo.Salons", "IX_Phone1Unique");
            DropIndex("dbo.Salons", "IX_NameUnique");
            DropIndex("dbo.Professions", "IX_NameUnique");
            DropIndex("dbo.Employees", new[] { "CompanyId" });
            DropIndex("dbo.Employees", new[] { "SalonId" });
            DropIndex("dbo.Employees", new[] { "BarberLevelId" });
            DropIndex("dbo.Employees", new[] { "ProfessionId" });
            DropIndex("dbo.Employees", "IX_EmployeeUnique");
            DropIndex("dbo.Companies", "IX_NameUnique");
            DropIndex("dbo.ServiceVolumes", "IX_NameUnique");
            DropIndex("dbo.Services", "IX_ServiceUnique");
            DropIndex("dbo.Trademarks", "IX_NameUnique");
            DropIndex("dbo.ProductVolumes", "IX_NameUnique");
            DropIndex("dbo.Productlines", "IX_NameUnique");
            DropIndex("dbo.Products", "IX_ProductUnique");
            DropIndex("dbo.PriceListUnits", "IX_NameUnique");
            DropIndex("dbo.CashRegCodes", new[] { "ServiceId" });
            DropIndex("dbo.CashRegCodes", new[] { "ProductId" });
            DropIndex("dbo.CashRegCodes", new[] { "PriceListUnitId" });
            DropIndex("dbo.CashRegCodes", "IX_CodeUnique");
            DropIndex("dbo.BarberLevels", "IX_NameUnique");
            DropTable("dbo.DelayedUpdateEmployees");
            DropTable("dbo.SalonTypes");
            DropTable("dbo.SalonStates");
            DropTable("dbo.Salons");
            DropTable("dbo.Professions");
            DropTable("dbo.Employees");
            DropTable("dbo.Companies");
            DropTable("dbo.ServiceVolumes");
            DropTable("dbo.Services");
            DropTable("dbo.Trademarks");
            DropTable("dbo.ProductVolumes");
            DropTable("dbo.Productlines");
            DropTable("dbo.Products");
            DropTable("dbo.PriceListUnits");
            DropTable("dbo.CashRegCodes");
            DropTable("dbo.BarberLevels");
        }
    }
}
