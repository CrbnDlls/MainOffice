namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductAndServiceLenth : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Products", "IX_ProductUnique");
            DropIndex("dbo.Services", "IX_ServiceUnique");
            AlterColumn("dbo.Products", "Name", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.Services", "Name", c => c.String(nullable: false, maxLength: 200));
            CreateIndex("dbo.Products", new[] { "Name", "TrademarkId", "ProductlineId", "ProductVolumeId" }, unique: true, name: "IX_ProductUnique");
            CreateIndex("dbo.Services", new[] { "Name", "ServiceVolumeId" }, unique: true, name: "IX_ServiceUnique");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Services", "IX_ServiceUnique");
            DropIndex("dbo.Products", "IX_ProductUnique");
            AlterColumn("dbo.Services", "Name", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Products", "Name", c => c.String(nullable: false, maxLength: 50));
            CreateIndex("dbo.Services", new[] { "Name", "ServiceVolumeId" }, unique: true, name: "IX_ServiceUnique");
            CreateIndex("dbo.Products", new[] { "Name", "TrademarkId", "ProductlineId", "ProductVolumeId" }, unique: true, name: "IX_ProductUnique");
        }
    }
}
