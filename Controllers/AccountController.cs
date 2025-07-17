using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Skipperu.Data;
using Skipperu.Data.Users.data;
using Skipperu.Dtos.ErrorHandling;
using Skipperu.Dtos.Users;
using Skipperu.Models.Accounts;
using Skipperu.Repos.Users;
namespace Skipperu.Controllers
{
    [ApiController]
    [Route("Accounts")]
    public class AspIdentityAccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signinManager;
        private readonly GlobalUserRepo _GlobalUserDBRepo;
        public AspIdentityAccountController (UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, 
            IGlobalUserRepo UsersRepo)
        {
            _GlobalUserDBRepo = (GlobalUserRepo)UsersRepo;
            _signinManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<string>> Register([FromForm]UserDataDto RegisterInformation)
        {
            var User = new IdentityUser { UserName = RegisterInformation.UserName, Email = RegisterInformation.UserEmail };

            if(await _userManager.FindByNameAsync(RegisterInformation.UserName) == null)
            {
                var result = await _userManager.CreateAsync(User,
                RegisterInformation.UserPassword);

                if (result.Succeeded)
                {
                    var AspIdentityUser = await _userManager.FindByNameAsync(RegisterInformation.UserName);

                    await _GlobalUserDBRepo.AddAsync(new GlobalUser { AspUser = AspIdentityUser, UserName = AspIdentityUser.NormalizedUserName });
                    await _GlobalUserDBRepo.SaveAsync();

                    await _userManager.AddClaimsAsync(User, GroupClaims.RoleUser.Value);

                    return JsonConvert.SerializeObject(new ResultMessage { Message = "Registration Successful", type = MessageTypes.SUCCESFUL });
                }

                List<ResultMessage> Errors = new List<ResultMessage>();
                foreach (var error in result.Errors)
                {
                    Errors.Add( new ResultMessage { Message = error.Description, type = MessageTypes.ERROR } );
                }


                return JsonConvert.SerializeObject(Errors);
            }

            else
            {
                return JsonConvert.SerializeObject( new ResultMessage { Message = "User Already Exists", type = MessageTypes.INFO } );
            }

        }


        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login([FromForm]UserDataDto SignInInfo)
        {
            string? ExceptionMessager = "" ;

            try {
                _signinManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;

                if (_GlobalUserDBRepo.GetByUserName(SignInInfo.UserName) != null)
                {
                    var AspIdentityUser = await _userManager.FindByNameAsync(SignInInfo.UserName);
                    var signInResult = await _signinManager.PasswordSignInAsync(AspIdentityUser, SignInInfo.UserPassword, true, false);

                    if (signInResult.Succeeded)
                    {
                        return JsonConvert.SerializeObject(new ResultMessage { Message = " Successful Sign in ", type = MessageTypes.SUCCESFUL } );
                    }
                }
                else
                {
                    return JsonConvert.SerializeObject(new ResultMessage { Message = " User Does Not Exist", type = MessageTypes.NOTFOUND });
                }

            }
            catch (Exception ex)
            {
                ExceptionMessager =  ex.Message;
            }

            return JsonConvert.SerializeObject(new ResultMessage { Message = ExceptionMessager, type = MessageTypes.INFO });
        }


        [HttpPost("AdminConfig25")]
        public async Task<ActionResult<string>> ConfigureAdmin([FromForm] UserDataDto SignInInfo)
        {
            string? ExceptionMessager = "";
            try
            {
                if (_GlobalUserDBRepo.GetByUserName(SignInInfo.UserName) != null)
                {
                    var AspIdentityUser = await _userManager.FindByNameAsync(SignInInfo.UserName);
                    await _userManager.AddClaimsAsync(AspIdentityUser, GroupClaims.RoleSuperAdmin.Value);

                    var signInResult = await _signinManager.PasswordSignInAsync(AspIdentityUser, SignInInfo.UserPassword, true, false);

                    if (signInResult.Succeeded)
                    {
                        return JsonConvert.SerializeObject(new ResultMessage { Message = " Successful Sign in ", type = MessageTypes.SUCCESFUL });
                    }
                }
                else
                {
                    return JsonConvert.SerializeObject(new ResultMessage { Message = " Register Admin User First ", type = MessageTypes.NOTFOUND });
                }
            }
            catch (Exception ex)
            {
                ExceptionMessager = ex.Message;
            }

            return JsonConvert.SerializeObject(new ResultMessage { Message = ExceptionMessager, type = MessageTypes.INFO });

        }


        //TODO: CONFIRMATION EMAIL 
        [HttpPost("GetConfirmationCode")]
        public async Task RequestConfirmationCode([FromForm] UserDataDto SignInInfo)
        {

            if (_GlobalUserDBRepo.GetByUserName(SignInInfo.UserName) != null)
            {
                var AspIdentityUser = await _userManager.FindByEmailAsync(SignInInfo.UserName);
                await _userManager.GenerateEmailConfirmationTokenAsync(AspIdentityUser);
            }
        }

        [HttpPost("ApplyCode")]
        public async Task ApplyConfirmationCode()
        {

        }
    }
}
