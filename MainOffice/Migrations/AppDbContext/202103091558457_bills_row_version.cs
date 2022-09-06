namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bills_row_version : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bills", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bills", "RowVersion");
        }
    }
}
