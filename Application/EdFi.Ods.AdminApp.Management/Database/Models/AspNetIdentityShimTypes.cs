#if !NET48
using Microsoft.AspNetCore.Identity;

namespace EdFi.Ods.AdminApp.Management.Database.Models
{
    public class IdentityUserClaim : IdentityUserClaim<string>
    {
    }

    public class IdentityUserLogin : IdentityUserLogin<string>
    {
    }

    public class IdentityUserRole : IdentityUserRole<string>
    {
    }
}
#endif
