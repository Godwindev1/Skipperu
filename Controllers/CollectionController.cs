using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Skipperu.Data.Repositories;
using Skipperu.Dtos.ErrorHandling;
using Skipperu.Models.Collections;
using Skipperu.Repos.Users;
using System.Net;

namespace Skipperu.Controllers
{
    [ApiController]
    [Authorize(policy: "IsUser")]
    [Route("Collections")]
    public class CollectionController : Controller
    {
        private readonly CollectionManagerModel collectionManagerModel;
        private readonly UserManager<IdentityUser> userManager;
        private readonly GlobalUserRepo _UsersRepo;
        private readonly CollectionsRepo _collectionsRepo;
        public CollectionController (UserManager<IdentityUser> inUserManager, ICollectionsRepo repos , IGlobalUserRepo UserRepo)
        {
            userManager = inUserManager;
            collectionManagerModel = new CollectionManagerModel(repos);
            _UsersRepo = (GlobalUserRepo)UserRepo;
            _collectionsRepo = (CollectionsRepo)repos;
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ResultMessage>> CreateRootFolder (string FolderName)
        {
            if(User.Identity != null)
            {
                var AspUser = await userManager.FindByNameAsync(User.Identity.Name);
                var GLobaluser = _UsersRepo.GetByUserName(AspUser.NormalizedUserName);

                return await collectionManagerModel.CreateRootFolder(GLobaluser, FolderName);
            }

            return new ResultMessage { Message = "User Is Not Signed in", type = MessageTypes.ERROR };
        }


     
        [HttpPost("CreateSubFolder")]

        public async Task<ActionResult<ResultMessage>> CreateFolder (string FolderName, string ParentHeirachyPath, string HeirachyPath)
        {
            if (User.Identity != null)
            {
                var AspUser = await userManager.FindByNameAsync(User.Identity.Name);
                var GLobaluser = _UsersRepo.GetByUserName(AspUser.NormalizedUserName);

                return await collectionManagerModel.CreateFolder(GLobaluser, ParentHeirachyPath, FolderName, HeirachyPath);
            }

            return new ResultMessage { Message = "User Is Not Signed in", type = MessageTypes.ERROR };
        }



        [HttpPost("DeleteFolder")]
        public async Task<ActionResult<ResultMessage>> DeleteFolder(string FolderPath)
        {
            if (User.Identity != null)
            {
                var AspUser = await userManager.FindByNameAsync(User.Identity.Name);
                var Globaluser = _UsersRepo.GetByUserName(AspUser.NormalizedUserName);

                return await  collectionManagerModel.DeleteFolder(Globaluser.GlobalUserID, FolderPath);
            }

            return new ResultMessage { Message = "User Is Not Signed in", type = MessageTypes.ERROR };

        }

        [HttpPost("ChangeFolderName")]
        public async Task<ActionResult<ResultMessage>> ChangeFolderName( string NewName, string FolderPath, string newFolderPath)
        {
            if (User.Identity != null)
            {
                var AspUser = await userManager.FindByNameAsync(User.Identity.Name);
                var Globaluser = _UsersRepo.GetByUserName(AspUser.NormalizedUserName);

                return await collectionManagerModel.ChangeFolderName(Globaluser.GlobalUserID, NewName, FolderPath, newFolderPath);
            }

            return new ResultMessage { Message = "User Is Not Signed in", type = MessageTypes.ERROR };

        }

        [HttpGet("GetSubFolders")]
        public async Task GetSubFolders(string FolderName, string FolderPath)
        {
            if (User.Identity != null)
            {
                var AspUser = await userManager.FindByNameAsync(User.Identity.Name);
                var Globaluser = _UsersRepo.GetByUserName(AspUser.NormalizedUserName);

                var result = await collectionManagerModel.GetAllSubFolders(FolderName, Globaluser.GlobalUserID, FolderPath);

                if(result == null)
                {
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await HttpContext.Response.WriteAsJsonAsync(new ResultMessage { Message = "Folder Des not Exist", type = MessageTypes.ERROR });
                }
                else
                {
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await HttpContext.Response.WriteAsJsonAsync(result);
                }

                    return;
            }


            HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await HttpContext.Response.WriteAsJsonAsync(new ResultMessage { Message = "User Is Not Signed in", type = MessageTypes.ERROR });
        }


        //TODO: enpoint For Changing Folders Parent 
        //Use Normalized Name FOr FOlders SO Users Cant Create Same Name With Different Case Combinations
    }
}
