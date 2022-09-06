namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceVolumeLenth : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ServiceVolumes", "IX_NameUnique");
            AlterColumn("dbo.ServiceVolumes", "Name", c => c.String(nullable: false, maxLength: 100));
            CreateIndex("dbo.ServiceVolumes", "Name", unique: true, name: "IX_NameUnique");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ServiceVolumes", "IX_NameUnique");
            AlterColumn("dbo.ServiceVolumes", "Name", c => c.String(nullable: false, maxLength: 50));
            CreateIndex("dbo.ServiceVolumes", "Name", unique: true, name: "IX_NameUnique");
        }
    }
}
