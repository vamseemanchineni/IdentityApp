using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Data
{
    public class DataContext : IdentityDbContext<AppUser, AppRole, int, IdentityUserClaim<int>,
                               AppUserRoleBridge, IdentityUserLogin<int>, IdentityRoleClaim<int>,IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<AppUser>().HasMany(ur=> ur.Roles).WithOne(ur => ur.User).HasForeignKey(ur => ur.UserId).IsRequired();
            builder.Entity<AppRole>().HasMany(ur=> ur.Users).WithOne(ur => ur.Role).HasForeignKey(ur => ur.RoleId).IsRequired();
  
        }
    }
}
