using Microsoft.AspNetCore.Identity;
using System;
using System.Collections;
using System.Collections.Generic;

namespace IdentityApp.Models
{
    public class AppUser : IdentityUser<int>
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        public ICollection<AppUserRoleBridge> Roles { get; set; }
    }
}
