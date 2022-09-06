namespace MainOffice.Migrations.ApplicationDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class user_salon : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "SalonId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "SalonId");
        }
    }
}
