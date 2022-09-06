namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prof_color : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BarberLevels", "Color", c => c.String());
            AddColumn("dbo.Professions", "Color", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Professions", "Color");
            DropColumn("dbo.BarberLevels", "Color");
        }
    }
}
