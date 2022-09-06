namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class maxprice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OperDayBillLines", "MaxPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OperDayBillLines", "MaxPrice");
        }
    }
}
