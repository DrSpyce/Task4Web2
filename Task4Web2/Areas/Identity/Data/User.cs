using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Task4Web2.Areas.Identity.Data;

// Add profile data for application users by adding properties to the User class
public class User : IdentityUser
{
    public string Name { get; set; } = default!;
    public virtual DateTime? LastLoginTime { get; set; }
    public virtual DateTime? RegistrationDate { get; set; }
    public string Status { get; set; } = "Active";
}

