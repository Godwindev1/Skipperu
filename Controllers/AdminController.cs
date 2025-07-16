using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Skipperu.Data;
using Skipperu.Data.Users.data;
using Skipperu.Dtos.Users;
using Skipperu.Models.Accounts;

namespace Skipperu.Controllers
{

    [ApiController]
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signinManager;
        private readonly UserAuthenticationDBcontext _DBctxt;
        public AdminController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, UserAuthenticationDBcontext Context)
        {
            _signinManager = signInManager;
            _userManager = userManager;
            _DBctxt = Context;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<string>> Register([FromForm] UserDataDto RegisterInformation)
        {
            var User = new IdentityUser { UserName = RegisterInformation.UserName, Email = RegisterInformation.UserEmail };

            var AspIdentityUser = _DBctxt.Users.Where(x => x.NormalizedUserName.Equals(User.UserName)).FirstOrDefault();

            if (AspIdentityUser == null)
            {
                var result = await _userManager.CreateAsync(User,
                RegisterInformation.UserPassword);


                if (result.Succeeded)
                {
                    //TODO Replace Raw DBcontxt with Irepo For Setting CGlbal User
                    AspIdentityUser = _DBctxt.Users.Where(x => x.NormalizedUserName.Equals(User.UserName)).FirstOrDefault();

                    _DBctxt.GlobalUsers.Add(new GlobalUser { AspUser = AspIdentityUser, UserName = AspIdentityUser.NormalizedUserName });
                    await _DBctxt.SaveChangesAsync();

                    await _userManager.AddClaimsAsync(User, GroupClaims.RoleSuperAdmin.Value);

                    //TODO: return Proper Message
                    return "Registered";
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }


            }



            //Update For Better Error Description
            return "failed";
        }
    }
}
