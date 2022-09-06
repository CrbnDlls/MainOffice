namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class client : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        FamilyName = c.String(maxLength: 50),
                        FathersName = c.String(maxLength: 50),
                        BirthDay = c.DateTime(storeType: "date"),
                        PhoneNumber = c.String(nullable: false, maxLength: 15),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.PhoneNumber, unique: true, name: "IX_ClientUnique");
            
            CreateTable(
                "dbo.ClientPhones",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PhoneNumber = c.String(nullable: false, maxLength: 15),
                        ClientId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId, cascadeDelete: true)
                .Index(t => t.ClientId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ClientPhones", "ClientId", "dbo.Clients");
            DropIndex("dbo.ClientPhones", new[] { "ClientId" });
            DropIndex("dbo.Clients", "IX_ClientUnique");
            DropTable("dbo.ClientPhones");
            DropTable("dbo.Clients");
        }
    }
}
