namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class operday_1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OperationDayArchives",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SalonId = c.Int(nullable: false),
                        OperationDate = c.DateTime(nullable: false, storeType: "date"),
                        OpenOperationPoint = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        OpenEmployeeId = c.Int(nullable: false),
                        OpenGeoLocation = c.String(),
                        CloseOperationPoint = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CloseEmployeeId = c.Int(nullable: false),
                        CloseGeoLocation = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.CloseEmployeeId, cascadeDelete: true)
                .ForeignKey("dbo.Employees", t => t.OpenEmployeeId)
                .ForeignKey("dbo.Salons", t => t.SalonId, cascadeDelete: true)
                .Index(t => t.SalonId)
                .Index(t => t.OpenEmployeeId)
                .Index(t => t.CloseEmployeeId);
            
            CreateTable(
                "dbo.OperationDayEmployeeArchives",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OperationDayArchiveId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        StartPoint = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        StartEmployeeId = c.Int(nullable: false),
                        EndPoint = c.DateTime(precision: 7, storeType: "datetime2"),
                        EndEmployeeId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EndEmployeeId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.OperationDayArchives", t => t.OperationDayArchiveId, cascadeDelete: true)
                .ForeignKey("dbo.Employees", t => t.StartEmployeeId)
                .Index(t => t.OperationDayArchiveId)
                .Index(t => t.EmployeeId)
                .Index(t => t.StartEmployeeId)
                .Index(t => t.EndEmployeeId);
            
            CreateTable(
                "dbo.OperationDayEmployees",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OperationDayId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        StartPoint = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        StartEmployeeId = c.Int(nullable: false),
                        EndPoint = c.DateTime(precision: 7, storeType: "datetime2"),
                        EndEmployeeId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EndEmployeeId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.OperationDays", t => t.OperationDayId, cascadeDelete: true)
                .ForeignKey("dbo.Employees", t => t.StartEmployeeId)
                .Index(t => t.OperationDayId)
                .Index(t => t.EmployeeId)
                .Index(t => t.StartEmployeeId)
                .Index(t => t.EndEmployeeId);
            
            CreateTable(
                "dbo.OperationDays",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SalonId = c.Int(nullable: false),
                        OperationDate = c.DateTime(storeType: "date"),
                        OpenOperationPoint = c.DateTime(precision: 7, storeType: "datetime2"),
                        OpenEmployeeId = c.Int(),
                        OpenGeoLocation = c.String(),
                        CloseOperationPoint = c.DateTime(precision: 7, storeType: "datetime2"),
                        CloseEmployeeId = c.Int(),
                        CloseGeoLocation = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.CloseEmployeeId, cascadeDelete: true)
                .ForeignKey("dbo.Employees", t => t.OpenEmployeeId)
                .ForeignKey("dbo.Salons", t => t.SalonId, cascadeDelete: true)
                .Index(t => t.SalonId)
                .Index(t => t.OpenEmployeeId)
                .Index(t => t.CloseEmployeeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OperationDayEmployees", "StartEmployeeId", "dbo.Employees");
            DropForeignKey("dbo.OperationDays", "SalonId", "dbo.Salons");
            DropForeignKey("dbo.OperationDayEmployees", "OperationDayId", "dbo.OperationDays");
            DropForeignKey("dbo.OperationDays", "OpenEmployeeId", "dbo.Employees");
            DropForeignKey("dbo.OperationDays", "CloseEmployeeId", "dbo.Employees");
            DropForeignKey("dbo.OperationDayEmployees", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.OperationDayEmployees", "EndEmployeeId", "dbo.Employees");
            DropForeignKey("dbo.OperationDayArchives", "SalonId", "dbo.Salons");
            DropForeignKey("dbo.OperationDayEmployeeArchives", "StartEmployeeId", "dbo.Employees");
            DropForeignKey("dbo.OperationDayEmployeeArchives", "OperationDayArchiveId", "dbo.OperationDayArchives");
            DropForeignKey("dbo.OperationDayEmployeeArchives", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.OperationDayEmployeeArchives", "EndEmployeeId", "dbo.Employees");
            DropForeignKey("dbo.OperationDayArchives", "OpenEmployeeId", "dbo.Employees");
            DropForeignKey("dbo.OperationDayArchives", "CloseEmployeeId", "dbo.Employees");
            DropIndex("dbo.OperationDays", new[] { "CloseEmployeeId" });
            DropIndex("dbo.OperationDays", new[] { "OpenEmployeeId" });
            DropIndex("dbo.OperationDays", new[] { "SalonId" });
            DropIndex("dbo.OperationDayEmployees", new[] { "EndEmployeeId" });
            DropIndex("dbo.OperationDayEmployees", new[] { "StartEmployeeId" });
            DropIndex("dbo.OperationDayEmployees", new[] { "EmployeeId" });
            DropIndex("dbo.OperationDayEmployees", new[] { "OperationDayId" });
            DropIndex("dbo.OperationDayEmployeeArchives", new[] { "EndEmployeeId" });
            DropIndex("dbo.OperationDayEmployeeArchives", new[] { "StartEmployeeId" });
            DropIndex("dbo.OperationDayEmployeeArchives", new[] { "EmployeeId" });
            DropIndex("dbo.OperationDayEmployeeArchives", new[] { "OperationDayArchiveId" });
            DropIndex("dbo.OperationDayArchives", new[] { "CloseEmployeeId" });
            DropIndex("dbo.OperationDayArchives", new[] { "OpenEmployeeId" });
            DropIndex("dbo.OperationDayArchives", new[] { "SalonId" });
            DropTable("dbo.OperationDays");
            DropTable("dbo.OperationDayEmployees");
            DropTable("dbo.OperationDayEmployeeArchives");
            DropTable("dbo.OperationDayArchives");
        }
    }
}
