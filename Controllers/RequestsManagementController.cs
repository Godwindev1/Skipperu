using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Skipperu.Data.Repositories;
using Skipperu.Data.Requests;
using Skipperu.Dtos.ErrorHandling;
using Skipperu.Models.Collections;
using Skipperu.Repos.Users;
using System.Net;
using Skipperu.Models.Requests;
using Skipperu.Dtos.RequestsInfo;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Skipperu.Data.Users.data;

namespace Skipperu.Controllers
{
    [ApiController]
    [Authorize(policy: "IsUser")]
    [Route("RequestsManagement")]
    public class RequestsManagementController : Controller
    {
        private readonly RequestsMangerModel ManagerModel;
        private readonly UserManager<IdentityUser> userManager;
        private readonly GlobalUserRepo _UsersRepo;
        private readonly CollectionsRepo _collectionsRepo;
        private readonly RequestsRepo _requestsRepo;
        private readonly Mapper _mapper;
        private readonly IMemoryCache memoryCache;


        public RequestsManagementController(UserManager<IdentityUser> inUserManager,
            IRequestsRepo RequestsRepo, ICollectionsRepo repos, IGlobalUserRepo UserRepo, IMapper MappingProfile, IMemoryCache Cache)
        {
            userManager = inUserManager;
            ManagerModel = new RequestsMangerModel(RequestsRepo, repos);
            _UsersRepo = (GlobalUserRepo)UserRepo;
            _collectionsRepo = (CollectionsRepo)repos;
            _requestsRepo = (RequestsRepo)RequestsRepo;
            _mapper = (Mapper)MappingProfile;
            memoryCache = Cache;
        }

        [HttpGet("GetRequests")]
        public async Task GetAllRequestsPerFolder(string FolderPath)
        {
            if (User.Identity != null)
            {
                var GlobalUser = memoryCache.Get<GlobalUser>(User.Identity.Name);

                if (GlobalUser != null)
                {
                    var result = await ManagerModel.GetAllRequestFromFolder(FolderPath, GlobalUser.GlobalUserID);

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

                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await HttpContext.Response.WriteAsJsonAsync(new ResultMessage { Message = "Backend Error Retrieving Cached User", type = MessageTypes.ERROR });



            }



        }


        [HttpPost("SaveRequest")]
        public async Task<ResultMessage> SaveRequest([FromForm] RequestDBinputDTO Request, string FolderPath)
        {
            if (User.Identity != null)
            {
                var GlobalUser = memoryCache.Get<GlobalUser>(User.Identity.Name);

                if (GlobalUser != null)
                {
                    return await ManagerModel.SaveRequestToFolder(_mapper.Map<RequestDBstore>(Request), FolderPath, GlobalUser.GlobalUserID);
                }

                return new ResultMessage { Message = "Backend Error Retrieving Cached User", type = MessageTypes.ERROR };
            }

            return new ResultMessage
            {
                Message = "User Is Not Signed in",
                type = MessageTypes.ERROR
            };
        }

        [HttpPost("DeleteRequest")]
        public async Task<ResultMessage> RemoveRequest(string FolderPath, string ReqeustName)
        {
            if (User.Identity != null)
            {
                var GlobalUser = memoryCache
                                    .Get<GlobalUser>(User.Identity.Name);

                if (GlobalUser != null)
                {
                    var result = await ManagerModel
                    .GetAllRequestFromFolder(FolderPath, GlobalUser.GlobalUserID);

                    var ID = result
                    .Where(x => x.RequestName == ReqeustName).FirstOrDefault().RequestID;

                    return await ManagerModel
                                    .DeleteRequestFromFolder(ID);
                }

                return new ResultMessage
                {
                    Message = "Backend Error Retrieving Cached User",
                    type = MessageTypes.ERROR
                };
            }

            return new ResultMessage
            {
                Message = "User Is Not Signed in",
                type = MessageTypes.ERROR
            };
        }

        [HttpPost("ChangeRequestName")]
        public async Task<ResultMessage> ChangeRequestName(string folderPath, string RequestName, string newname)
        {
            if (User.Identity != null)
            {
                var GlobalUser = memoryCache
                                    .Get<GlobalUser>(User.Identity.Name);

                if (GlobalUser != null)
                {
                   return await  ManagerModel.ChangeRequestName(RequestName, newname, folderPath, GlobalUser.GlobalUserID);
                }

                return new ResultMessage
                {
                    Message = "Backend Error Retrieving Cached User",
                    type = MessageTypes.ERROR
                };
            }

            return new ResultMessage
            {
                Message = "User Is Not Signed in",
                type = MessageTypes.ERROR
            };
        }


    };

}
