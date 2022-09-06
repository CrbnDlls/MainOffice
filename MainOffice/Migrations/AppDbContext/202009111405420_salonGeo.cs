namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salonGeo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Salons", "Longitude", c => c.Double(nullable: false));
            AddColumn("dbo.Salons", "Latitude", c => c.Double(nullable: false));
            DropColumn("dbo.Salons", "SalonKey");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Salons", "SalonKey", c => c.String(nullable: false));
            DropColumn("dbo.Salons", "Latitude");
            DropColumn("dbo.Salons", "Longitude");
        }
    }
}
