using System.ComponentModel.DataAnnotations.Schema;

namespace Skipperu.Data.Users.data.ExternalAuth.data
{
    public class GoogleUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string GoogleIdentityKey { get; set; }
        public string NormalizedEmail { get; set; }

        public string UserEmail { get; set; }

        public string NormalizedUserName { get; set; }

        public string UserName { get; set; }
    }
}
