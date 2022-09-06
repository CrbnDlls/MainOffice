namespace MainOffice.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class AppDbContext : DbContext
    {
        // Your context has been configured to use a 'AppDbContext' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'MainOffice.Models.AppDbContext' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'AppDbContext' 
        // connection string in the application configuration file.
        public AppDbContext()
            : base("name=AppDbContext")
        {
        }

        
        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
        public System.Data.Entity.DbSet<MainOffice.Models.SalonType> SalonTypes { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.SalonState> SalonStates { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.Salon> Salons { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.SalonPrinter> SalonPrinters { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.Company> Companies { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.Profession> Professions { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.BarberLevel> BarberLevels { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.Employee> Employees { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.Trademark> Trademarks { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.ProductVolume> ProductVolumes { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.Productline> Productlines { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.Product> Products { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.ServiceVolume> ServiceVolumes { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.Service> Services { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.PriceListUnit> PriceListUnits { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.CashRegCode> CashRegCodes { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.DelayedUpdateEmployee> DelayedUpdateEmployees { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.DelayedUpdateCashRegCode> DelayedUpdateCashRegCodes { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.Client> Clients { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.ClientPhone> ClientPhones { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.Bill> Bills { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.BillLine> BillLines { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.Promotion> Promotions { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.OperationDay> OperationDays { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.OperationDayArchive> OperationDayArchives { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.OperationDayEmployee> OperationDayEmployees { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.OperationDayEmployeeArchive> OperationDayEmployeeArchives { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.OperDayBill> OperDayBills { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.OperDayBillLine> OperDayBillLines { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.OperDayBillPrint> OperDayBillPrints { get; set; }

        public System.Data.Entity.DbSet<MainOffice.Models.PStatus> PStatuses { get; set; }

    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}