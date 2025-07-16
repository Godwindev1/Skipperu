using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Skipperu.Data;
using Skipperu.Data.Requests;
using Skipperu.Data.Users.data;
using Skipperu.Data.Users.data.ExternalAuth.data;
using Skipperu.Dtos.RequestsInfo;
using Skipperu.Models.Requests.client.proxy;
using Skipperu.Tests.param.binders;
using System.Globalization;

namespace Skipperu.Tests
{

    [ApiController]
    [Route("TEST")]
    [Authorize(policy: "IsAdmin")]
    public class TestController : ControllerBase
    {
        RequestClientProxyModel Model;

        private readonly UserAuthenticationDBcontext __Context;
        private readonly Mapper __MappingCtxt;

        public TestController(UserAuthenticationDBcontext Context, IMapper mappingContext)
        {
            __MappingCtxt = (Mapper)mappingContext;
            __Context = Context;
            Model = new RequestClientProxyModel();
            GlobalUser TestAdmin = new GlobalUser { ExternalAuthUser = new ExternalAuthUser { } };

            string sql = $"SELECT * FROM GlobalUsers WHERE ExternalAuthFK = 'ec5f7376-179f-42ae-bb7f-ebef9257cee3' ";

     
        }

        [HttpGet("get")]
        public async Task<ActionResult<string>> TestGetRequest([ModelBinder(binderType: typeof(RequestInfoBinder))]RequestInfo requestContxt)
        {
           return await Model.ForwardRequest(requestContxt, RestSharp.Method.Get);
        }

        [HttpPost("post")]
        public async Task<ActionResult<string>> TestPostRequest([ModelBinder(binderType: typeof(RequestInfoBinder))] RequestInfo requestContxt)
        {
            return await Model.ForwardRequest(requestContxt, RestSharp.Method.Post);
        }

        [HttpPut("put")]
        public async Task<ActionResult<string>> TestPutRequest([ModelBinder(binderType: typeof(RequestInfoBinder))] RequestInfo requestContxt)
        {
            return await Model.ForwardRequest(requestContxt, RestSharp.Method.Put);
        }

        [HttpPatch("patch")]
        public async Task<ActionResult<string>> TestPatchRequest([ModelBinder(binderType: typeof(RequestInfoBinder))] RequestInfo requestContxt)
        {
            return await Model.ForwardRequest(requestContxt, RestSharp.Method.Patch);
        }


        [HttpOptions("options")]
        public async Task<ActionResult<string>> TestoptionsRequest([ModelBinder(binderType: typeof(RequestInfoBinder))] RequestInfo requestContxt)
        {
            return await Model.ForwardRequest(requestContxt, RestSharp.Method.Options);
        }

        [HttpHead("head")]
        public async Task<ActionResult<string>> TestHeadRequest([ModelBinder(binderType: typeof(RequestInfoBinder))] RequestInfo requestContxt)
        {
            return await Model.ForwardRequest(requestContxt, RestSharp.Method.Head);
        }

        [HttpDelete("delete")]
        public async Task<ActionResult<string>> TestDeleteRequest([ModelBinder(binderType: typeof(RequestInfoBinder))] RequestInfo requestContxt)
        {
            return await Model.ForwardRequest(requestContxt, RestSharp.Method.Delete);
        }

        [HttpPost("SaveRequest")]
        public async Task<ActionResult<string>> TestSaveRequest([FromForm]RequestDBinputDTO requestContxt, [FromQuery]string FolderName)
        {
            string sql = $"SELECT * FROM GlobalRequestCollection WHERE FolderName = '{FolderName}' ";

            try {
                var Savedcollection = __Context.GlobalRequestCollection.FromSqlRaw(sql).FirstOrDefault();

                if (Savedcollection == null)
                {
                    var req = __MappingCtxt.Map<RequestDBstore>(requestContxt);
                    var newCollection = new Collection
                    {
                        GlobalUserID = "a047e9e6-d389-4852-bdc8-0c11fc77b70a",
                        FolderName = FolderName
                    };

                    var newRequest = new RequestDBstore
                    {
                        BodyJSON = req.BodyJSON,
                        HeaderJSON = req.HeaderJSON,
                        Endpoint = req.Endpoint,
                        QueryParametersJSON = req.QueryParametersJSON,
                        Host = req.Host,

                        ParentFolder = newCollection // ✅ This tells EF the relationship
                    };

                    __Context.requests.Add(newRequest);

                    int ? Res = __Context.SaveChanges();
                
                    if(Res != null)
                    {
                        return "Saved";
                    }
                }
                else
                {
                    var req = __MappingCtxt.Map<RequestDBstore>(requestContxt);
               

                    var newRequest = new RequestDBstore
                    {
                        BodyJSON = req.BodyJSON,
                        HeaderJSON = req.HeaderJSON,
                        Endpoint = req.Endpoint,
                        QueryParametersJSON = req.QueryParametersJSON,
                        Host = req.Host,

                        ParentFolder = Savedcollection // ✅ This tells EF the relationship
                    };

                    __Context.requests.Add(newRequest);

                    int? Res = __Context.SaveChanges();

                    if (Res != null)
                    {
                        return "Saved";
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "Failed";
        }
    }
}
