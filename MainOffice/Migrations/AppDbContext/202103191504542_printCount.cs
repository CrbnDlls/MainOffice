namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printCount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bills", "PrintCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bills", "PrintCount");
        }
    }
}
