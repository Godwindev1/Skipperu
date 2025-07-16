namespace Skipperu.Models.Accounts
{
    using Microsoft.AspNetCore.Identity;
    using System.Security.Claims;
    using KVPRoleClaim = KeyValuePair<string, List<System.Security.Claims.Claim>>;

    public class Claims
    {
        public static Claim Admin = new Claim("Role", "Admin");
        public static Claim User = new Claim("Role", "User");

    }

    public class GroupClaims
    {
        public static KVPRoleClaim RoleSuperAdmin = new KVPRoleClaim("Admin",
            new List<Claim> { Claims.Admin });

        public static KVPRoleClaim RoleUser = new KVPRoleClaim("User",
           new List<Claim> { Claims.User });

    
    }

}
