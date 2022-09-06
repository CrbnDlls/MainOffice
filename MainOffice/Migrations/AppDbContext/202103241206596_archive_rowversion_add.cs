namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class archive_rowversion_add : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OperationDayArchives", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.OperationDayEmployeeArchives", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OperationDayEmployeeArchives", "RowVersion");
            DropColumn("dbo.OperationDayArchives", "RowVersion");
        }
    }
}
