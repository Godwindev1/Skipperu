using Skipperu.Models.data.structs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using Azure;



namespace Skipperu.Controllers.param.binders
{
    using Desrializer = Newtonsoft.Json.JsonConvert;
    using KvpList = Dictionary<string, string>;

    public class RequestInfoBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext context)
        {
            if(context.HttpContext.Request.HasFormContentType || context.HttpContext.Request.HasJsonContentType())
            {
                RequestInfo ParsedInfo = new();
                KvpList? SkipperuRequestHandlerBodyData = new KvpList();

                if (context.HttpContext.Request.HasFormContentType)
                {
                    var FormCollection = await context.HttpContext.Request.ReadFormAsync();

                    foreach ( var Item in FormCollection)
                    {
                        SkipperuRequestHandlerBodyData.Add(Item.Key, Item.Value);
                    }
                }

                else if((context.HttpContext.Request.HasJsonContentType()))
                {
                    using var reader = new StreamReader(context.HttpContext.Request.Body);
                    var body = await reader.ReadToEndAsync();
                    SkipperuRequestHandlerBodyData = Desrializer.DeserializeObject<KvpList>(body);
                }
    

                if(SkipperuRequestHandlerBodyData?.Any() != true)
                {
                    SkipperuRequestHandlerBodyData = new KvpList();
                    return;
                }

                if (SkipperuRequestHandlerBodyData.ContainsKey("UserHeaderKVP"))
                {
                    ParsedInfo.HeaderKVP = Desrializer.DeserializeObject<KvpList>(SkipperuRequestHandlerBodyData["UserHeaderKVP"]);
                    SkipperuRequestHandlerBodyData.Remove("UserHeaderKVP");
                }

                if (SkipperuRequestHandlerBodyData.ContainsKey("UserQueryKVP"))
                {
                    ParsedInfo.QueryParametersKVP = Desrializer.DeserializeObject<KvpList>(SkipperuRequestHandlerBodyData["UserQueryKVP"]);
                    SkipperuRequestHandlerBodyData.Remove("UserQueryKVP");
                }


                if (SkipperuRequestHandlerBodyData.ContainsKey("UserBodyKVP"))
                {
                    ParsedInfo.BodyKVP = Desrializer.DeserializeObject<KvpList>(SkipperuRequestHandlerBodyData["UserBodyKVP"]);
                    SkipperuRequestHandlerBodyData.Remove("UserBodyKVP");
                }

                if (SkipperuRequestHandlerBodyData.ContainsKey("UserTargetedHost"))
                {
                    ParsedInfo.Host = SkipperuRequestHandlerBodyData["UserTargetedHost"];
                    SkipperuRequestHandlerBodyData.Remove("UserBodyKVP");
                }

                if (SkipperuRequestHandlerBodyData.ContainsKey("UserEndpoint"))
                {
                    ParsedInfo.Endpoint = SkipperuRequestHandlerBodyData["UserEndpoint"];
                    SkipperuRequestHandlerBodyData.Remove("UserEndpoint");
                }


                context.Result = ModelBindingResult.Success(ParsedInfo);
            }

            else
            {
                context.Result = ModelBindingResult.Failed();
            }

                return ;

        }
    }

}
