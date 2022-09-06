namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class oper_empl_pin : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OperationDayEmployees", "pin", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OperationDayEmployees", "pin");
        }
    }
}
