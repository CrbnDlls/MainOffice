namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OperDayBillPrints",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Token = c.String(),
                        Count = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OperDayBills", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.SalonPrinters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        SalonId = c.Int(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Salons", t => t.SalonId, cascadeDelete: true)
                .Index(t => new { t.Name, t.SalonId }, unique: true, name: "IX_PrinterUnique");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SalonPrinters", "SalonId", "dbo.Salons");
            DropForeignKey("dbo.OperDayBillPrints", "Id", "dbo.OperDayBills");
            DropIndex("dbo.SalonPrinters", "IX_PrinterUnique");
            DropIndex("dbo.OperDayBillPrints", new[] { "Id" });
            DropTable("dbo.SalonPrinters");
            DropTable("dbo.OperDayBillPrints");
        }
    }
}
