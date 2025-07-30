using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Skipperu.Data.Repositories;
using Skipperu.Data.Requests;
using Skipperu.Data.Users.data;
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
        private readonly IMemoryCache _memoryCache;
        public CollectionController(UserManager<IdentityUser> inUserManager, ICollectionsRepo repos, IGlobalUserRepo UserRepo, IMemoryCache memoryCache)
        {
            userManager = inUserManager;
            collectionManagerModel = new CollectionManagerModel(repos);
            _UsersRepo = (GlobalUserRepo)UserRepo;
            _collectionsRepo = (CollectionsRepo)repos;
            _memoryCache = memoryCache;
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ResultMessage>> CreateRootFolder (string FolderName)
        {
            FolderName = FolderName.ToUpperInvariant();

            if (User.Identity != null)
            {
                var GlobalUser = _memoryCache.Get<GlobalUser>(User.Identity.Name);

                if (GlobalUser != null)
                {
                    return await collectionManagerModel.CreateRootFolder(GlobalUser, FolderName);
                }

                return new ResultMessage
                {
                    Message = "Backend Error Retrieving User From Cache",
                    type = MessageTypes.ERROR
                };

            }

            return new ResultMessage
            {
                Message = "User Is Not Signed in",
                type = MessageTypes.ERROR
            };
        }

        
     
        [HttpPost("CreateSubFolder")]

        public async Task<ActionResult<ResultMessage>> CreateFolder (string FolderName, string ParentHeirachyPath, string HeirachyPath)
        {
            FolderName = FolderName.ToUpperInvariant();

            if (User.Identity != null)
            {
                var GlobalUser = _memoryCache.Get<GlobalUser>(User.Identity.Name);

                if (GlobalUser != null)
                {
                    return await collectionManagerModel
                    .CreateFolder(GlobalUser, ParentHeirachyPath, FolderName, HeirachyPath);
                }

                return new ResultMessage
                {
                    Message = "Backend Error Retrieving User From Cache",
                    type = MessageTypes.ERROR
                };

            }

            return new ResultMessage
            {
                Message = "User Is Not Signed in",
                type = MessageTypes.ERROR
            };
        }



        [HttpPost("DeleteFolder")]
        public async Task<ActionResult<ResultMessage>> DeleteFolder(string FolderPath)
        {
            if (User.Identity != null)
            {
                var GlobalUser = _memoryCache.Get<GlobalUser>(User.Identity.Name);

                if (GlobalUser != null)
                {
                    return await collectionManagerModel
                    .DeleteFolder(GlobalUser.GlobalUserID, FolderPath);
                }

                return new ResultMessage
                {
                    Message = "Backend Error Retrieving User From Cache",
                    type = MessageTypes.ERROR
                };
            }

            return new ResultMessage
            {
                Message = "User Is Not Signed in",
                type = MessageTypes.ERROR
            };

        }

        [HttpPost("ChangeFolderName")]
        public async Task<ActionResult<ResultMessage>> ChangeFolderName( string NewName, string FolderPath, string newFolderPath)
        {
            NewName = NewName.ToUpperInvariant();
            
            if (User.Identity != null)
            {
                var GlobalUser = _memoryCache.Get<GlobalUser>(User.Identity.Name);

                if (GlobalUser != null)
                {
                    return await collectionManagerModel
                    .ChangeFolderName(GlobalUser.GlobalUserID, NewName, FolderPath, newFolderPath);
                }

                return new ResultMessage
                {
                    Message = "Backend Error Retrieving User From Cache",
                    type = MessageTypes.ERROR
                };
            }

            return new ResultMessage
            {
                Message = "User Is Not Signed in",
                type = MessageTypes.ERROR
            };

        }

        [HttpGet("GetSubFolders")]
        public async Task GetSubFolders(string FolderName, string FolderPath)
        {
            FolderName = FolderName.ToUpperInvariant();
            if (User.Identity != null)
            {
                var GlobalUser = _memoryCache.Get<GlobalUser>(User.Identity.Name);

                if (GlobalUser != null)
                {
                    var result = await collectionManagerModel.GetAllSubFolders(FolderName, GlobalUser.GlobalUserID, FolderPath);

                    if (result == null)
                    {
                        HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        await HttpContext.Response.WriteAsJsonAsync(new ResultMessage { Message = "Folder Des not Exist", type = MessageTypes.ERROR });

                        return;
                    }
                    else
                    {
                        HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                        await HttpContext.Response.WriteAsJsonAsync(result);

                        return;
                    }
                }

                HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await HttpContext.Response.WriteAsJsonAsync(new ResultMessage
                {
                    Message = "Backend Error Retrieving User From Cache",
                    type = MessageTypes.ERROR
                });


            }


            HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await HttpContext.Response.WriteAsJsonAsync(new ResultMessage { Message = "User Is Not Signed in", type = MessageTypes.ERROR });
        }

        [HttpGet("GetRootFolders")]
        public async Task GetRootFolders()
        {
            if (User.Identity != null)
            {

                var GlobalUser = _memoryCache.Get<GlobalUser>(User.Identity.Name);

                if (GlobalUser != null)
                {

                    var result = await collectionManagerModel.GetAllRootFoldersForUser(GlobalUser.GlobalUserID);

                    if (result == null)
                    {
                        HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        await HttpContext.Response.WriteAsJsonAsync(new ResultMessage { Message = "Folder Des not Exist", type = MessageTypes.ERROR });

                        return;
                    }
                    else
                    {
                        HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                        await HttpContext.Response.WriteAsJsonAsync(result);

                        return;
                    }
                }

                HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await HttpContext.Response.WriteAsJsonAsync(new ResultMessage
                {
                    Message = "Backend Error Retrieving User From Cache",
                    type = MessageTypes.ERROR
                });
            }
            
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await HttpContext.Response.WriteAsJsonAsync(new ResultMessage { Message = "User Is Not Signed in", type = MessageTypes.ERROR });
        }


        //TODO: enpoint For Changing Folders Parent 
        //Use Normalized Name FOr FOlders SO Users Cant Create Same Name With Different Case Combinations
        //Change Error COdes Based ON success or Failure, Currently Errors Return Json with 200k
    }



}
