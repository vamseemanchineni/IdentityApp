using Microsoft.AspNetCore.Identity;

namespace IdentityApp.Models
{
    public class AppUserRoleBridge : IdentityUserRole<int>
    {
        public AppUser User { get; set; }
        public AppRole Role { get; set; }
    }
}
