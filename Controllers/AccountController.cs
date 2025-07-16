using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Skipperu.Data;
using Skipperu.Dtos.Users;
using Skipperu.Models.Accounts;
using Skipperu.Data.Users.data;
using Microsoft.AspNetCore.Authentication;
namespace Skipperu.Controllers
{
    [ApiController]
    [Route("Accounts")]
    public class AspIdentityAccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signinManager;
        private readonly UserAuthenticationDBcontext _DBctxt;
        public AspIdentityAccountController (UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, UserAuthenticationDBcontext Context)
        {
            _signinManager = signInManager;
            _userManager = userManager;
            _DBctxt = Context;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<string>> Register([FromForm]UserDataDto RegisterInformation)
        {
            var User = new IdentityUser { UserName = RegisterInformation.UserName, Email = RegisterInformation.UserEmail };

            var AspIdentityUser = _DBctxt.Users.Where(x => x.NormalizedUserName.Equals(User.UserName)).FirstOrDefault();

            if(AspIdentityUser != null)
            {
                var result = await _userManager.CreateAsync(User,
                RegisterInformation.UserPassword);


                if (result.Succeeded)
                {
                    //TODO Replace Raw DBcontxt with Irepo For Setting CGlbal User
                    AspIdentityUser = _DBctxt.Users.Where(x => x.NormalizedUserName.Equals(User.UserName)).FirstOrDefault();

                    _DBctxt.GlobalUsers.Add(new GlobalUser { AspUser = AspIdentityUser, UserName = AspIdentityUser.NormalizedUserName });
                    await _DBctxt.SaveChangesAsync();

                    await _userManager.AddClaimsAsync(User, GroupClaims.RoleUser.Value);

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


        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login([FromForm]UserDataDto SignInInfo)
        {
            try {
                var AspIdentityUser = _DBctxt.Users.Where(x => x.NormalizedUserName.Equals(SignInInfo.UserName)).FirstOrDefault();
                _signinManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;
                var signInResult =  await _signinManager.PasswordSignInAsync(AspIdentityUser, SignInInfo.UserPassword, false,  false);

                if(signInResult.Succeeded)
                {
                        return "Signed ID";
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return "Not Allowed To sign in";
        }

        [HttpPost("GetConfirmationCode")]
        public async Task RequestConfirmationCode()
        {

        }

        [HttpPost("ApplyCode")]
        public async Task ApplyConfirmationCode()
        {

        }
    }
}
