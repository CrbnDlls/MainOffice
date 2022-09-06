namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class operday2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OperationDayEmployeeArchives", "EndEmployeeId", "dbo.Employees");
            DropIndex("dbo.OperationDayEmployeeArchives", new[] { "EndEmployeeId" });
            AlterColumn("dbo.OperationDayEmployeeArchives", "EndPoint", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.OperationDayEmployeeArchives", "EndEmployeeId", c => c.Int(nullable: false));
            CreateIndex("dbo.OperationDayEmployeeArchives", "EndEmployeeId");
            AddForeignKey("dbo.OperationDayEmployeeArchives", "EndEmployeeId", "dbo.Employees", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OperationDayEmployeeArchives", "EndEmployeeId", "dbo.Employees");
            DropIndex("dbo.OperationDayEmployeeArchives", new[] { "EndEmployeeId" });
            AlterColumn("dbo.OperationDayEmployeeArchives", "EndEmployeeId", c => c.Int());
            AlterColumn("dbo.OperationDayEmployeeArchives", "EndPoint", c => c.DateTime(precision: 7, storeType: "datetime2"));
            CreateIndex("dbo.OperationDayEmployeeArchives", "EndEmployeeId");
            AddForeignKey("dbo.OperationDayEmployeeArchives", "EndEmployeeId", "dbo.Employees", "Id");
        }
    }
}
