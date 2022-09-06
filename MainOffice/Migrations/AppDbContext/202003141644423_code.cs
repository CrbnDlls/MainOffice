namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class code : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DelayedUpdateCashRegCodes",
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
                        CashRegCodeId = c.Int(),
                        UpdateDate = c.DateTime(storeType: "date"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CashRegCodes", t => t.CashRegCodeId)
                .ForeignKey("dbo.PriceListUnits", t => t.PriceListUnitId)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .ForeignKey("dbo.Services", t => t.ServiceId)
                .Index(t => t.Code, unique: true, name: "IX_CodeUnique")
                .Index(t => t.PriceListUnitId)
                .Index(t => t.ProductId)
                .Index(t => t.ServiceId)
                .Index(t => t.CashRegCodeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DelayedUpdateCashRegCodes", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.DelayedUpdateCashRegCodes", "ProductId", "dbo.Products");
            DropForeignKey("dbo.DelayedUpdateCashRegCodes", "PriceListUnitId", "dbo.PriceListUnits");
            DropForeignKey("dbo.DelayedUpdateCashRegCodes", "CashRegCodeId", "dbo.CashRegCodes");
            DropIndex("dbo.DelayedUpdateCashRegCodes", new[] { "CashRegCodeId" });
            DropIndex("dbo.DelayedUpdateCashRegCodes", new[] { "ServiceId" });
            DropIndex("dbo.DelayedUpdateCashRegCodes", new[] { "ProductId" });
            DropIndex("dbo.DelayedUpdateCashRegCodes", new[] { "PriceListUnitId" });
            DropIndex("dbo.DelayedUpdateCashRegCodes", "IX_CodeUnique");
            DropTable("dbo.DelayedUpdateCashRegCodes");
        }
    }
}
