using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Skipperu.Data;
using Skipperu.Data.Repositories;
using Skipperu.Data.Requests;
using Skipperu.Data.Users.data;
using Skipperu.Data.Users.data.ExternalAuth.data;
using Skipperu.Dtos.RequestsInfo;
using Skipperu.Models.Requests.client.proxy;
using Skipperu.Tests.param.binders;
using System.Globalization;

namespace Skipperu.Tests
{
    //TODO: FOLDERS MANAGEMENT, REQUESTS MANAGEMENT 

    [ApiController]
    [Route("TEST")]
    [Authorize(policy: "IsAdmin")]
    public class TestController : ControllerBase
    {
        RequestClientProxyModel Model;

        private readonly Mapper __MappingCtxt;
        private readonly RequestsRepo _RequestsRepository;
        private readonly CollectionsRepo _CollectionRepository;

        public TestController( IRequestsRepo RequestRepo, ICollectionsRepo CollectionRepo, IMapper mappingContext)
        {
            _RequestsRepository = (RequestsRepo)RequestRepo;
            _CollectionRepository = (CollectionsRepo)CollectionRepo;

            Model = new RequestClientProxyModel();
            GlobalUser TestAdmin = new GlobalUser { ExternalAuthUser = new ExternalAuthUser { } };

            string sql = $"SELECT * FROM GlobalUsers WHERE ExternalAuthFK = 'ec5f7376-179f-42ae-bb7f-ebef9257cee3' ";

     
        }

        [HttpGet("get")]
        public async Task<ActionResult<string>> TestGetRequest([ModelBinder(binderType: typeof(RequestInfoBinder))]RequestCallInfo requestContxt)
        {
           return await Model.ForwardRequest(requestContxt, RestSharp.Method.Get);
        }

        [HttpPost("post")]
        public async Task<ActionResult<string>> TestPostRequest([ModelBinder(binderType: typeof(RequestInfoBinder))] RequestCallInfo requestContxt)
        {
            return await Model.ForwardRequest(requestContxt, RestSharp.Method.Post);
        }

        [HttpPut("put")]
        public async Task<ActionResult<string>> TestPutRequest([ModelBinder(binderType: typeof(RequestInfoBinder))] RequestCallInfo requestContxt)
        {
            return await Model.ForwardRequest(requestContxt, RestSharp.Method.Put);
        }

        [HttpPatch("patch")]
        public async Task<ActionResult<string>> TestPatchRequest([ModelBinder(binderType: typeof(RequestInfoBinder))] RequestCallInfo requestContxt)
        {
            return await Model.ForwardRequest(requestContxt, RestSharp.Method.Patch);
        }

        [Authorize(policy: "IsUser")]
        [HttpOptions("options")]
        public async Task<ActionResult<string>> TestoptionsRequest([ModelBinder(binderType: typeof(RequestInfoBinder))] RequestCallInfo requestContxt)
        {
            return await Model.ForwardRequest(requestContxt, RestSharp.Method.Options);
        }

        [HttpHead("head")]
        public async Task<ActionResult<string>> TestHeadRequest([ModelBinder(binderType: typeof(RequestInfoBinder))] RequestCallInfo requestContxt)
        {
            return await Model.ForwardRequest(requestContxt, RestSharp.Method.Head);
        }

        [HttpDelete("delete")]
        public async Task<ActionResult<string>> TestDeleteRequest([ModelBinder(binderType: typeof(RequestInfoBinder))] RequestCallInfo requestContxt)
        {
            return await Model.ForwardRequest(requestContxt, RestSharp.Method.Delete);
        }

        [HttpPost("SaveRequest")]
        public async Task<ActionResult<string>> TestSaveRequest([FromForm]RequestDBinputDTO requestContxt, [FromQuery]string FolderName)
        {
            return "";
        }
    }
}
