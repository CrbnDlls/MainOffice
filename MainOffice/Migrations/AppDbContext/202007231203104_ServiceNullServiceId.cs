namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceNullServiceId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Services", "ServiceVolumeId", "dbo.ServiceVolumes");
            DropIndex("dbo.Services", "IX_ServiceUnique");
            AlterColumn("dbo.Services", "ServiceVolumeId", c => c.Int());
            CreateIndex("dbo.Services", new[] { "Name", "ServiceVolumeId" }, unique: true, name: "IX_ServiceUnique");
            AddForeignKey("dbo.Services", "ServiceVolumeId", "dbo.ServiceVolumes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Services", "ServiceVolumeId", "dbo.ServiceVolumes");
            DropIndex("dbo.Services", "IX_ServiceUnique");
            AlterColumn("dbo.Services", "ServiceVolumeId", c => c.Int(nullable: false));
            CreateIndex("dbo.Services", new[] { "Name", "ServiceVolumeId" }, unique: true, name: "IX_ServiceUnique");
            AddForeignKey("dbo.Services", "ServiceVolumeId", "dbo.ServiceVolumes", "Id", cascadeDelete: true);
        }
    }
}
