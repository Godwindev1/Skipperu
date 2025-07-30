using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RestSharp;
using Skipperu.Dtos.RequestsInfo;

namespace Skipperu.Models.Requests.client.proxy
{
    public partial class RequestClientProxyModel
    {
        private readonly ILogger logger;
        public RequestClientProxyModel()
        {   
        }

        public async Task<ActionResult<string>> ForwardRequest(RequestCallInfo RequestContext, Method HttpVerb)
        {

            try {
                if (RequestContext != null)
                {
                    RestClient _requestsForwarder = new RestClient(RequestContext.Host);

                    RestRequest Request = GetRequest(RequestContext, HttpVerb);
                    RestResponse Result = await _requestsForwarder.ExecuteAsync(Request);

                    if (Result.IsSuccessful && Result.Content != null)
                    {
                        return Result.Content;
                    }
                }

                else
                {
                    return new BadRequestResult();
                }
            }

            catch (Exception ex)
            {
                //TODO: ADD ERROR logging
                return new BadRequestResult();
            }



            return "";

        }

    }

    //TODO: Matching version Of this Class Will Exist On the Frontend To Allow Using the Users System A Forwarder For Calling Requests
}
