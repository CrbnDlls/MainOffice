namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_PStatus : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OperDayBills", "PStatusId", "dbo.PStatus");
            DropIndex("dbo.OperDayBills", new[] { "PStatusId" });
            AlterColumn("dbo.OperDayBills", "PStatusId", c => c.Int());
            CreateIndex("dbo.OperDayBills", "PStatusId");
            AddForeignKey("dbo.OperDayBills", "PStatusId", "dbo.PStatus", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OperDayBills", "PStatusId", "dbo.PStatus");
            DropIndex("dbo.OperDayBills", new[] { "PStatusId" });
            AlterColumn("dbo.OperDayBills", "PStatusId", c => c.Int(nullable: false));
            CreateIndex("dbo.OperDayBills", "PStatusId");
            AddForeignKey("dbo.OperDayBills", "PStatusId", "dbo.PStatus", "Id", cascadeDelete: true);
        }
    }
}
