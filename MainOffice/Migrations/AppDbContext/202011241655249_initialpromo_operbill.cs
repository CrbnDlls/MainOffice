namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialpromo_operbill : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OperDayBills", "InitialPromo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OperDayBills", "InitialPromo");
        }
    }
}
