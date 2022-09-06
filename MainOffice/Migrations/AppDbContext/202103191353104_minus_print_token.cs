namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class minus_print_token : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.OperDayBillPrints", "Token");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OperDayBillPrints", "Token", c => c.String());
        }
    }
}
