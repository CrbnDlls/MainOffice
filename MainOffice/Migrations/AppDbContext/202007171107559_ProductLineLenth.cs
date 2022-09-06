namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductLineLenth : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Productlines", "IX_NameUnique");
            AlterColumn("dbo.Productlines", "Name", c => c.String(nullable: false, maxLength: 200));
            CreateIndex("dbo.Productlines", "Name", unique: true, name: "IX_NameUnique");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Productlines", "IX_NameUnique");
            AlterColumn("dbo.Productlines", "Name", c => c.String(nullable: false, maxLength: 50));
            CreateIndex("dbo.Productlines", "Name", unique: true, name: "IX_NameUnique");
        }
    }
}
