namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salonIp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Salons", "IP", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Salons", "IP");
        }
    }
}
