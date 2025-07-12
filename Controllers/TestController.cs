using Skipperu.Controllers.param.binders;
using Skipperu.Models.data.structs;
using Skipperu.Models.Requests.client.proxy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;

namespace Skipperu.Controllers
{

    [ApiController]
    [Route("TEST")]
    public class TestController : ControllerBase
    {
        RequestClientProxyModel Model;
        
        public TestController()
        {
            Model = new RequestClientProxyModel();
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
    }
}
