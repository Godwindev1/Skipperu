using System.ComponentModel.DataAnnotations.Schema;

namespace Skipperu.Data.Users.data.ExternalAuth.data
{
    public class ExternalAuthUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string PrimaryKey { get; set; }
    }
}
