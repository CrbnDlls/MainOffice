namespace MainOffice.Migrations.ApplicationDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class User_EmployeeId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "EmployeeId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "EmployeeId");
        }
    }
}
