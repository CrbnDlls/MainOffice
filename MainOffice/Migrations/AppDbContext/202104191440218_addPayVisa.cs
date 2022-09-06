namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPayVisa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OperDayBills", "PayVisaId", c => c.Int());
            CreateIndex("dbo.OperDayBills", "PayVisaId");
            AddForeignKey("dbo.OperDayBills", "PayVisaId", "dbo.Employees", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OperDayBills", "PayVisaId", "dbo.Employees");
            DropIndex("dbo.OperDayBills", new[] { "PayVisaId" });
            DropColumn("dbo.OperDayBills", "PayVisaId");
        }
    }
}
