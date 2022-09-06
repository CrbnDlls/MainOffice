namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class timespan_to_datetime_operbill : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Bills", "StartTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Bills", "EndTime", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.OperDayBills", "StartTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.OperDayBills", "EndTime", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OperDayBills", "EndTime", c => c.Time(precision: 7));
            AlterColumn("dbo.OperDayBills", "StartTime", c => c.Time(nullable: false, precision: 7));
            AlterColumn("dbo.Bills", "EndTime", c => c.Time(precision: 7));
            AlterColumn("dbo.Bills", "StartTime", c => c.Time(nullable: false, precision: 7));
        }
    }
}
