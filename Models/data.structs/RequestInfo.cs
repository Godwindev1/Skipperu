using Skipperu.Controllers.param.binders;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Skipperu.Models.data.structs
{
    [ModelBinder(binderType: typeof(RequestInfoBinder))]
    public class RequestInfo
    {
        //Request Line
        [Required]
        public string Host { get; set; }
        [Required]
        public string Endpoint { get; set; }

        public Dictionary<string, string>? QueryParametersKVP { get; set; }

        //Request Headers
        public Dictionary<string, string>? HeaderKVP { get; set; }

        //Request Body
        public Dictionary<string, string> BodyKVP { get; set; }

    }
}


