namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class staff_concurrency : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BarberLevels", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.Employees", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.DelayedUpdateEmployees", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.Professions", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Professions", "RowVersion");
            DropColumn("dbo.DelayedUpdateEmployees", "RowVersion");
            DropColumn("dbo.Employees", "RowVersion");
            DropColumn("dbo.BarberLevels", "RowVersion");
        }
    }
}
