namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bill_concurency : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OperDayBills", "Locked", c => c.Boolean(nullable: false));
            AddColumn("dbo.OperDayBills", "WhoLockedId", c => c.Int());
            AddColumn("dbo.OperDayBills", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            CreateIndex("dbo.OperDayBills", "WhoLockedId");
            AddForeignKey("dbo.OperDayBills", "WhoLockedId", "dbo.Employees", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OperDayBills", "WhoLockedId", "dbo.Employees");
            DropIndex("dbo.OperDayBills", new[] { "WhoLockedId" });
            DropColumn("dbo.OperDayBills", "RowVersion");
            DropColumn("dbo.OperDayBills", "WhoLockedId");
            DropColumn("dbo.OperDayBills", "Locked");
        }
    }
}
