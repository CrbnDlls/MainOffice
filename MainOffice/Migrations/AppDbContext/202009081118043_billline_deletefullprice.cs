namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class billline_deletefullprice : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.BillLines", "FullPrice");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BillLines", "FullPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
