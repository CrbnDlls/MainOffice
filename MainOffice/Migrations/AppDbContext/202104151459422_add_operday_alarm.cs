namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_operday_alarm : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OperationDays", "Alarm", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OperationDays", "Alarm");
        }
    }
}
