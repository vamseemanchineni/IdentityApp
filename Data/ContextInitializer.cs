using API.Utility;
using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityApp.Data
{
    public static class ContextInitializer
    {
        public static async Task InitializeAsync(DataContext context, UserManager<AppUser> userManager)
        {
            if(context.Database.GetPendingMigrations().Count() > 0)
            {
                await context.Database.MigrateAsync();
            }
            if(!userManager.Users.Any())
            {
                var vams = new AppUser
                {
                    UserName = "vams",
                    Name = "Vamsee",
                  Email = "vams@example.com",
                  EmailConfirmed = true,
                  LockoutEnabled = true,
                };
                await userManager.CreateAsync(vams, StaticDetails.DefaultPassword);
                var megh = new AppUser
                {
                    UserName = "megh",
                    Name = "Meghaaa",
                    Email = "megh@example.com",
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                };
                await userManager.CreateAsync(megh, StaticDetails.DefaultPassword);
                var tej = new AppUser
                {
                    UserName = "tej",
                    Name = "Teja",
                    Email = "teja@example.com",
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                };
                await userManager.CreateAsync(tej, StaticDetails.DefaultPassword);
                var vrithik = new AppUser
                {
                    UserName = "vrithik",
                    Name = "vrithika",
                    Email = "vrithika@example.com",
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                };
                await userManager.CreateAsync(vrithik, StaticDetails.DefaultPassword);
            }
        }
    }
}
