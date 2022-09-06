namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PriceListUnitLenth : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PriceListUnits", "IX_NameUnique");
            AlterColumn("dbo.PriceListUnits", "Name", c => c.String(nullable: false, maxLength: 100));
            CreateIndex("dbo.PriceListUnits", "Name", unique: true, name: "IX_NameUnique");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PriceListUnits", "IX_NameUnique");
            AlterColumn("dbo.PriceListUnits", "Name", c => c.String(nullable: false, maxLength: 50));
            CreateIndex("dbo.PriceListUnits", "Name", unique: true, name: "IX_NameUnique");
        }
    }
}
