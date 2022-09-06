namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class delatedPricelistunits : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.EmployeePriceListUnits", "Employee_Id", "dbo.Employees");
            DropForeignKey("dbo.EmployeePriceListUnits", "PriceListUnit_Id", "dbo.PriceListUnits");
            DropIndex("dbo.EmployeePriceListUnits", new[] { "Employee_Id" });
            DropIndex("dbo.EmployeePriceListUnits", new[] { "PriceListUnit_Id" });
            AddColumn("dbo.PriceListUnits", "DelayedUpdateEmployee_Id", c => c.Int());
            AddColumn("dbo.PriceListUnits", "Employee_Id", c => c.Int());
            AddColumn("dbo.Employees", "PriceListUnit_Id", c => c.Int());
            AddColumn("dbo.Employees", "PriceListUnit_Id1", c => c.Int());
            CreateIndex("dbo.PriceListUnits", "DelayedUpdateEmployee_Id");
            CreateIndex("dbo.PriceListUnits", "Employee_Id");
            CreateIndex("dbo.Employees", "PriceListUnit_Id");
            CreateIndex("dbo.Employees", "PriceListUnit_Id1");
            AddForeignKey("dbo.PriceListUnits", "DelayedUpdateEmployee_Id", "dbo.DelayedUpdateEmployees", "Id");
            AddForeignKey("dbo.PriceListUnits", "Employee_Id", "dbo.Employees", "Id");
            AddForeignKey("dbo.Employees", "PriceListUnit_Id", "dbo.PriceListUnits", "Id");
            AddForeignKey("dbo.Employees", "PriceListUnit_Id1", "dbo.PriceListUnits", "Id");
            DropTable("dbo.EmployeePriceListUnits");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.EmployeePriceListUnits",
                c => new
                    {
                        Employee_Id = c.Int(nullable: false),
                        PriceListUnit_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Employee_Id, t.PriceListUnit_Id });
            
            DropForeignKey("dbo.Employees", "PriceListUnit_Id1", "dbo.PriceListUnits");
            DropForeignKey("dbo.Employees", "PriceListUnit_Id", "dbo.PriceListUnits");
            DropForeignKey("dbo.PriceListUnits", "Employee_Id", "dbo.Employees");
            DropForeignKey("dbo.PriceListUnits", "DelayedUpdateEmployee_Id", "dbo.DelayedUpdateEmployees");
            DropIndex("dbo.Employees", new[] { "PriceListUnit_Id1" });
            DropIndex("dbo.Employees", new[] { "PriceListUnit_Id" });
            DropIndex("dbo.PriceListUnits", new[] { "Employee_Id" });
            DropIndex("dbo.PriceListUnits", new[] { "DelayedUpdateEmployee_Id" });
            DropColumn("dbo.Employees", "PriceListUnit_Id1");
            DropColumn("dbo.Employees", "PriceListUnit_Id");
            DropColumn("dbo.PriceListUnits", "Employee_Id");
            DropColumn("dbo.PriceListUnits", "DelayedUpdateEmployee_Id");
            CreateIndex("dbo.EmployeePriceListUnits", "PriceListUnit_Id");
            CreateIndex("dbo.EmployeePriceListUnits", "Employee_Id");
            AddForeignKey("dbo.EmployeePriceListUnits", "PriceListUnit_Id", "dbo.PriceListUnits", "Id", cascadeDelete: true);
            AddForeignKey("dbo.EmployeePriceListUnits", "Employee_Id", "dbo.Employees", "Id", cascadeDelete: true);
        }
    }
}
