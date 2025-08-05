using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Skipperu.Data.Users.data;
using Skipperu.Data.Users.data.ExternalAuth.data;
using Skipperu.Dtos.ErrorHandling;
using System.Security.Claims;

namespace Skipperu.Controllers.AccountControllers
{
    public partial class AspIdentityAccountController : Controller
    {

        [HttpGet("GoogleAuth")]
        public async Task GoogleChallengeAuthentication()
        {
            try
            {
                await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties { IsPersistent = true, RedirectUri = "Accounts/RegisterGoogleUser" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Message: {ex.Message}");
            }

        }

        [HttpGet("RegisterGoogleUser")]
        public async Task<ActionResult<ResultMessage>> RegisterNewUser()
        {
            await HttpContext.AuthenticateAsync();

            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    string EmailCLaim = User.Claims
                        .Where(x => x.Type == ClaimTypes.Email)
                        .ToList()
                        .FirstOrDefault().Value;

                    string NameClaim = User.Claims
                        .Where(x => x.Type == ClaimTypes.Name)
                        .ToList()
                        .FirstOrDefault().Value;


                    var NewGoogleUser = new GoogleUser
                    {
                        UserName = NameClaim,
                        NormalizedUserName = NameClaim.ToUpperInvariant(),
                        UserEmail = EmailCLaim,
                        NormalizedEmail = EmailCLaim.ToUpperInvariant(),
                    };

                    if (_GlobalUserDBRepo.GetByNormalizedUserEMail(EmailCLaim) == null)
                    {
                        await _GoogleUserRepo.CreateAsync(NewGoogleUser);
                        await _GoogleUserRepo.SaveAsync();

                        var Globaluser = new GlobalUser
                        {
                            NormalizedUserEmail = NewGoogleUser.NormalizedEmail,
                            GoogleAuth = NewGoogleUser
                        };

                        await _GlobalUserDBRepo.AddAsync(Globaluser);
                        await _GoogleUserRepo.SaveAsync();
                    }

                    //TODO: Redirect To Application Dashboard

                    return new ResultMessage { Message = "Google Authentication Successful", type = MessageTypes.AUTHENTICATED };
                }
                else
                {
                    return new ResultMessage { Message = "Google Authentication Unsuccessful", type = MessageTypes.ERROR };
                }

            }
            catch (Exception ex)
            {
                return new ResultMessage { Message = ex.Message, type = MessageTypes.ERROR };
            }

        }



    }
}
