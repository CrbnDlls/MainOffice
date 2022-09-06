namespace MainOffice.Migrations.AppDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class code1 : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.CashRegCodes", name: "IX_CodeUnique", newName: "IX_CashRegCodeUnique");
            RenameIndex(table: "dbo.DelayedUpdateCashRegCodes", name: "IX_CodeUnique", newName: "IX_CashRegCodeUnique");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.DelayedUpdateCashRegCodes", name: "IX_CashRegCodeUnique", newName: "IX_CodeUnique");
            RenameIndex(table: "dbo.CashRegCodes", name: "IX_CashRegCodeUnique", newName: "IX_CodeUnique");
        }
    }
}
