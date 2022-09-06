namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printersysName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalonPrinters", "SystemPrinterName", c => c.String(nullable: false, maxLength: 50));
            CreateIndex("dbo.SalonPrinters", new[] { "SystemPrinterName", "SalonId" }, unique: true, name: "IX_PrinterSysNameUnique");
        }
        
        public override void Down()
        {
            DropIndex("dbo.SalonPrinters", "IX_PrinterSysNameUnique");
            DropColumn("dbo.SalonPrinters", "SystemPrinterName");
        }
    }
}
