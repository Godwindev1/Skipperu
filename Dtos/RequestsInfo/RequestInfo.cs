using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Skipperu.Tests.param.binders;

namespace Skipperu.Dtos.RequestsInfo
{
    [ModelBinder(binderType: typeof(RequestInfoBinder))]
    public class RequestInfo
    {
        [Required]
        public string Host { get; set; }
        [Required]
        public string Endpoint { get; set; }

        public Dictionary<string, string>? QueryParametersKVP { get; set; }
        public Dictionary<string, string>? HeaderKVP { get; set; }
        public Dictionary<string, string> BodyKVP { get; set; }

    }
}


