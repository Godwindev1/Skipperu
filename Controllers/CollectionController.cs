using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Skipperu.Data.Repositories;
using Skipperu.Dtos.ErrorHandling;
using Skipperu.Models.Collections;
using Skipperu.Repos.Users;

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

        public async Task<ActionResult<ResultMessage>> DeleteFolder(string FolderName, string FolderPath)
        {
            if (User.Identity != null)
            {
                var AspUser = await userManager.FindByNameAsync(User.Identity.Name);
                var Globaluser = _UsersRepo.GetByUserName(AspUser.NormalizedUserName);

                return await  collectionManagerModel.DeleteFolder(Globaluser.GlobalUserID, FolderName, FolderPath);
            }

            return new ResultMessage { Message = "User Is Not Signed in", type = MessageTypes.ERROR };

        }

    }
}
