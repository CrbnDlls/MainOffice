namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class operday_bill : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OperDayBills",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BillNumber = c.Int(nullable: false),
                        OperationDayEmployeeId = c.Int(nullable: false),
                        StartTime = c.Time(nullable: false, precision: 7),
                        EndTime = c.Time(precision: 7),
                        BO = c.Boolean(nullable: false),
                        ClientId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .ForeignKey("dbo.OperationDayEmployees", t => t.OperationDayEmployeeId, cascadeDelete: true)
                .Index(t => t.OperationDayEmployeeId)
                .Index(t => t.ClientId);
            
            CreateTable(
                "dbo.OperDayBillLines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OperDayBillId = c.Int(nullable: false),
                        CashRegCode = c.Int(nullable: false),
                        ProductOrServiceName = c.String(nullable: false),
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
                .ForeignKey("dbo.OperDayBills", t => t.OperDayBillId, cascadeDelete: true)
                .Index(t => t.OperDayBillId)
                .Index(t => t.AdminVisaId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OperDayBills", "OperationDayEmployeeId", "dbo.OperationDayEmployees");
            DropForeignKey("dbo.OperDayBills", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.OperDayBillLines", "OperDayBillId", "dbo.OperDayBills");
            DropForeignKey("dbo.OperDayBillLines", "AdminVisaId", "dbo.Employees");
            DropIndex("dbo.OperDayBillLines", new[] { "AdminVisaId" });
            DropIndex("dbo.OperDayBillLines", new[] { "OperDayBillId" });
            DropIndex("dbo.OperDayBills", new[] { "ClientId" });
            DropIndex("dbo.OperDayBills", new[] { "OperationDayEmployeeId" });
            DropTable("dbo.OperDayBillLines");
            DropTable("dbo.OperDayBills");
        }
    }
}
