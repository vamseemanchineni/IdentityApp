using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace IdentityApp.Models
{
    public class AppRole : IdentityRole<int>
    {
        public ICollection<AppUserRoleBridge> Users { get; set; }
    }
}
