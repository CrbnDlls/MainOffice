using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MainOffice.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            userIdentity.AddClaim(new Claim(ClaimTypes.Locality, this.Locality));
            userIdentity.AddClaim(new Claim(ClaimTypes.SerialNumber, this.EmployeeId.HasValue ? EmployeeId.Value.ToString() : "0"));
            // Здесь добавьте утверждения пользователя
            return userIdentity;
        }
        
        public string Locality { get; set; }
        public int? EmployeeId { get; set; }
        public int? SalonId { get; set; }
    }

    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName) : base(roleName) { }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public override DbSet<TEntity> Set<TEntity>()
        {
            return base.Set<TEntity>();
        }

        public override DbSet Set(Type entityType)
        {
            return base.Set(entityType);
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
        public System.Data.Entity.DbSet<MainOffice.Models.Locality> Localities { get; set; }

    }
}