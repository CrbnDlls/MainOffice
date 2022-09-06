namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class delatedPricelistunits2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PriceListUnits", "DelayedUpdateEmployee_Id", "dbo.DelayedUpdateEmployees");
            DropForeignKey("dbo.PriceListUnits", "Employee_Id", "dbo.Employees");
            DropForeignKey("dbo.Employees", "PriceListUnit_Id", "dbo.PriceListUnits");
            DropForeignKey("dbo.Employees", "PriceListUnit_Id1", "dbo.PriceListUnits");
            DropIndex("dbo.PriceListUnits", new[] { "DelayedUpdateEmployee_Id" });
            DropIndex("dbo.PriceListUnits", new[] { "Employee_Id" });
            DropIndex("dbo.Employees", new[] { "PriceListUnit_Id" });
            DropIndex("dbo.Employees", new[] { "PriceListUnit_Id1" });
            CreateTable(
                "dbo.EmployeePriceListUnits",
                c => new
                    {
                        Employee_Id = c.Int(nullable: false),
                        PriceListUnit_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Employee_Id, t.PriceListUnit_Id })
                .ForeignKey("dbo.Employees", t => t.Employee_Id, cascadeDelete: true)
                .ForeignKey("dbo.PriceListUnits", t => t.PriceListUnit_Id, cascadeDelete: true)
                .Index(t => t.Employee_Id)
                .Index(t => t.PriceListUnit_Id);
            
            CreateTable(
                "dbo.DelayedUpdateEmployeePriceListUnits",
                c => new
                    {
                        DelayedUpdateEmployee_Id = c.Int(nullable: false),
                        PriceListUnit_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.DelayedUpdateEmployee_Id, t.PriceListUnit_Id })
                .ForeignKey("dbo.DelayedUpdateEmployees", t => t.DelayedUpdateEmployee_Id, cascadeDelete: true)
                .ForeignKey("dbo.PriceListUnits", t => t.PriceListUnit_Id, cascadeDelete: true)
                .Index(t => t.DelayedUpdateEmployee_Id)
                .Index(t => t.PriceListUnit_Id);
            
            DropColumn("dbo.PriceListUnits", "DelayedUpdateEmployee_Id");
            DropColumn("dbo.PriceListUnits", "Employee_Id");
            DropColumn("dbo.Employees", "PriceListUnit_Id");
            DropColumn("dbo.Employees", "PriceListUnit_Id1");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Employees", "PriceListUnit_Id1", c => c.Int());
            AddColumn("dbo.Employees", "PriceListUnit_Id", c => c.Int());
            AddColumn("dbo.PriceListUnits", "Employee_Id", c => c.Int());
            AddColumn("dbo.PriceListUnits", "DelayedUpdateEmployee_Id", c => c.Int());
            DropForeignKey("dbo.DelayedUpdateEmployeePriceListUnits", "PriceListUnit_Id", "dbo.PriceListUnits");
            DropForeignKey("dbo.DelayedUpdateEmployeePriceListUnits", "DelayedUpdateEmployee_Id", "dbo.DelayedUpdateEmployees");
            DropForeignKey("dbo.EmployeePriceListUnits", "PriceListUnit_Id", "dbo.PriceListUnits");
            DropForeignKey("dbo.EmployeePriceListUnits", "Employee_Id", "dbo.Employees");
            DropIndex("dbo.DelayedUpdateEmployeePriceListUnits", new[] { "PriceListUnit_Id" });
            DropIndex("dbo.DelayedUpdateEmployeePriceListUnits", new[] { "DelayedUpdateEmployee_Id" });
            DropIndex("dbo.EmployeePriceListUnits", new[] { "PriceListUnit_Id" });
            DropIndex("dbo.EmployeePriceListUnits", new[] { "Employee_Id" });
            DropTable("dbo.DelayedUpdateEmployeePriceListUnits");
            DropTable("dbo.EmployeePriceListUnits");
            CreateIndex("dbo.Employees", "PriceListUnit_Id1");
            CreateIndex("dbo.Employees", "PriceListUnit_Id");
            CreateIndex("dbo.PriceListUnits", "Employee_Id");
            CreateIndex("dbo.PriceListUnits", "DelayedUpdateEmployee_Id");
            AddForeignKey("dbo.Employees", "PriceListUnit_Id1", "dbo.PriceListUnits", "Id");
            AddForeignKey("dbo.Employees", "PriceListUnit_Id", "dbo.PriceListUnits", "Id");
            AddForeignKey("dbo.PriceListUnits", "Employee_Id", "dbo.Employees", "Id");
            AddForeignKey("dbo.PriceListUnits", "DelayedUpdateEmployee_Id", "dbo.DelayedUpdateEmployees", "Id");
        }
    }
}
