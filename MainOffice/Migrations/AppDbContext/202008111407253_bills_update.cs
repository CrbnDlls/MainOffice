namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bills_update : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.DelayedUpdateEmployeePriceListUnits", newName: "PriceListUnitDelayedUpdateEmployees");
            RenameTable(name: "dbo.EmployeePriceListUnits", newName: "PriceListUnitEmployees");
            DropPrimaryKey("dbo.PriceListUnitDelayedUpdateEmployees");
            DropPrimaryKey("dbo.PriceListUnitEmployees");
            CreateTable(
                "dbo.BillLines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BillId = c.Int(nullable: false),
                        CashRegCode = c.Int(nullable: false),
                        ProductOrServiceName = c.String(nullable: false),
                        FullPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Int(nullable: false),
                        Promotion = c.String(),
                        SellPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InsertDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Cancel = c.Boolean(nullable: false),
                        CancelDateTime = c.DateTime(precision: 7, storeType: "datetime2"),
                        AdminVisaId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.AdminVisaId)
                .ForeignKey("dbo.Bills", t => t.BillId, cascadeDelete: true)
                .Index(t => t.BillId)
                .Index(t => t.AdminVisaId);
            
            CreateTable(
                "dbo.Bills",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BillNumber = c.Int(nullable: false),
                        IssueDate = c.DateTime(nullable: false, storeType: "date"),
                        SalonId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        StartTime = c.Time(nullable: false, precision: 7),
                        EndTime = c.Time(precision: 7),
                        BO = c.Boolean(nullable: false),
                        ClientId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("dbo.Salons", t => t.SalonId, cascadeDelete: true)
                .Index(t => t.SalonId)
                .Index(t => t.EmployeeId)
                .Index(t => t.ClientId);
            
            AddPrimaryKey("dbo.PriceListUnitDelayedUpdateEmployees", new[] { "PriceListUnit_Id", "DelayedUpdateEmployee_Id" });
            AddPrimaryKey("dbo.PriceListUnitEmployees", new[] { "PriceListUnit_Id", "Employee_Id" });
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bills", "SalonId", "dbo.Salons");
            DropForeignKey("dbo.Bills", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Bills", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.BillLines", "BillId", "dbo.Bills");
            DropForeignKey("dbo.BillLines", "AdminVisaId", "dbo.Employees");
            DropIndex("dbo.Bills", new[] { "ClientId" });
            DropIndex("dbo.Bills", new[] { "EmployeeId" });
            DropIndex("dbo.Bills", new[] { "SalonId" });
            DropIndex("dbo.BillLines", new[] { "AdminVisaId" });
            DropIndex("dbo.BillLines", new[] { "BillId" });
            DropPrimaryKey("dbo.PriceListUnitEmployees");
            DropPrimaryKey("dbo.PriceListUnitDelayedUpdateEmployees");
            DropTable("dbo.Bills");
            DropTable("dbo.BillLines");
            AddPrimaryKey("dbo.PriceListUnitEmployees", new[] { "Employee_Id", "PriceListUnit_Id" });
            AddPrimaryKey("dbo.PriceListUnitDelayedUpdateEmployees", new[] { "DelayedUpdateEmployee_Id", "PriceListUnit_Id" });
            RenameTable(name: "dbo.PriceListUnitEmployees", newName: "EmployeePriceListUnits");
            RenameTable(name: "dbo.PriceListUnitDelayedUpdateEmployees", newName: "DelayedUpdateEmployeePriceListUnits");
        }
    }
}
