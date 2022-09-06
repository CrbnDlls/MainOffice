namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class promo_operbill : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OperDayBills", "ClientPromo", c => c.String());
            AddColumn("dbo.OperDayBills", "VisaPromoId", c => c.Int());
            CreateIndex("dbo.OperDayBills", "VisaPromoId");
            AddForeignKey("dbo.OperDayBills", "VisaPromoId", "dbo.Employees", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OperDayBills", "VisaPromoId", "dbo.Employees");
            DropIndex("dbo.OperDayBills", new[] { "VisaPromoId" });
            DropColumn("dbo.OperDayBills", "VisaPromoId");
            DropColumn("dbo.OperDayBills", "ClientPromo");
        }
    }
}
