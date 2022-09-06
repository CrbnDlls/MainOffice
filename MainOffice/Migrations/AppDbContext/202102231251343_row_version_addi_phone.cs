namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class row_version_addi_phone : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClientPhones", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClientPhones", "RowVersion");
        }
    }
}
