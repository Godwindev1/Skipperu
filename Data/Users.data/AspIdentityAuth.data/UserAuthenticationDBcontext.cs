using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Skipperu.Data.Users.data.AspIdentityAuth.data
{
    public class UserAuthenticationDBcontext : IdentityDbContext<IdentityUser>
    {
        public UserAuthenticationDBcontext(DbContextOptions options) : base(options)
        {
        }

        protected UserAuthenticationDBcontext()
        {
        }
    }
}
