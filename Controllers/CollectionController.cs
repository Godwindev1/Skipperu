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


        //TODO: Proper DSA For Representing Parent Heirachy
        //Proper Heirachal Search And Creation
        [HttpPost("CreateSubFolder")]

        public async Task<ActionResult<ResultMessage>> CreateFolder (string FolderName, string ParentName)
        {
            if (User.Identity != null)
            {
                var AspUser = await userManager.FindByNameAsync(User.Identity.Name);
                var GLobaluser = _UsersRepo.GetByUserName(AspUser.NormalizedUserName);

                var Collection = (await _collectionsRepo.
                    GetAllByUserAsync(GLobaluser.GlobalUserID)).Where(x => x.FolderName == ParentName).FirstOrDefault();

                //TODO: Example situation, Rootfolder->DefaultFolder->Subfolder exists
                //and RootFolder->DefaultFolder2->Subfolder exists 
                //if Create Folder where ParentName=SubFolder And FOlderName=Any 
                //Both Of the Described FOlders Above Meet these Description And Since First Or default is Used It Always Adds
                //it To The First in the List 

                //Possible FIX Is Tracking the Hierachy Down To the Root To Make Sure THeres always A Distinction


                return await collectionManagerModel.CreateFolder(GLobaluser, Collection.FolderRootID, FolderName);
            }

            return new ResultMessage { Message = "User Is Not Signed in", type = MessageTypes.ERROR };
        }

    }
}
