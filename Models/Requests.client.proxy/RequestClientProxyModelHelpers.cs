using Azure;
using Skipperu.Models.data.structs;
using Microsoft.IdentityModel.Tokens;
using RestSharp;

namespace Skipperu.Models.Requests.client.proxy
{
    public partial class RequestClientProxyModel
    {
        public RestRequest GetRequest(RequestInfo RequestContext, Method Method)
        {

            RestRequest Request = new RestRequest(RequestContext.Endpoint, Method);
            Request.AddHeader("Content-Type", "application/json");

            if (!RequestContext.HeaderKVP.Any() == true) { Request.AddHeaders(RequestContext.HeaderKVP); }
            if (!RequestContext.BodyKVP.Any() == true) { Request.AddJsonBody(RequestContext.BodyKVP); }

            if (!RequestContext.QueryParametersKVP.Any() == true)
            {
                foreach (var param in RequestContext.QueryParametersKVP)
                {
                    Request.AddQueryParameter(param.Key, param.Value);
                }
            }

            return Request;

        }
    }
}
