namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prof_order : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BarberLevels", "OrderNumber", c => c.Int());
            AddColumn("dbo.Professions", "OrderNumber", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Professions", "OrderNumber");
            DropColumn("dbo.BarberLevels", "OrderNumber");
        }
    }
}
