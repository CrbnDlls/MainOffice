namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class row_version_almost_all : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.PriceListUnits", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.CashRegCodes", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.DelayedUpdateCashRegCodes", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.Products", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.Productlines", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.ProductVolumes", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.Trademarks", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.Services", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.ServiceVolumes", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.Salons", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.SalonStates", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.SalonTypes", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.Clients", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "RowVersion");
            DropColumn("dbo.SalonTypes", "RowVersion");
            DropColumn("dbo.SalonStates", "RowVersion");
            DropColumn("dbo.Salons", "RowVersion");
            DropColumn("dbo.ServiceVolumes", "RowVersion");
            DropColumn("dbo.Services", "RowVersion");
            DropColumn("dbo.Trademarks", "RowVersion");
            DropColumn("dbo.ProductVolumes", "RowVersion");
            DropColumn("dbo.Productlines", "RowVersion");
            DropColumn("dbo.Products", "RowVersion");
            DropColumn("dbo.DelayedUpdateCashRegCodes", "RowVersion");
            DropColumn("dbo.CashRegCodes", "RowVersion");
            DropColumn("dbo.PriceListUnits", "RowVersion");
            DropColumn("dbo.Companies", "RowVersion");
        }
    }
}
