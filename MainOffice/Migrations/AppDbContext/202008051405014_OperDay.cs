namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OperDay : DbMigration
    {
        public override void Up()
        {
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
            
            AddColumn("dbo.Salons", "SalonKey", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmployeePriceListUnits", "PriceListUnit_Id", "dbo.PriceListUnits");
            DropForeignKey("dbo.EmployeePriceListUnits", "Employee_Id", "dbo.Employees");
            DropIndex("dbo.EmployeePriceListUnits", new[] { "PriceListUnit_Id" });
            DropIndex("dbo.EmployeePriceListUnits", new[] { "Employee_Id" });
            DropColumn("dbo.Salons", "SalonKey");
            DropTable("dbo.EmployeePriceListUnits");
        }
    }
}
