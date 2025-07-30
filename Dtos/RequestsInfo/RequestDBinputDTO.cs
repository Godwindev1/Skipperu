using System.ComponentModel.DataAnnotations;

namespace Skipperu.Dtos.RequestsInfo
{
    public class RequestDBinputDTO
    {
        public string UserTargetedHost { get; set; }
        [Required]
        public string UserEndpoint { get; set; }

        public string? UserQueryKVP { get; set; }

        public string? UserHeaderKVP { get; set; }

        public string? UserBodyKVP { get; set; }

        public string RequestName { get; set;  }
    }
}
