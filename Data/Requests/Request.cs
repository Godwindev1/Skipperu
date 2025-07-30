using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skipperu.Data.Requests
{
    public class RequestDBstore
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string RequestID { get; set; }
        [Required]
        public string Host { get; set; }
        [Required]
        public string Endpoint { get; set; }

        public string? QueryParametersJSON { get; set; }

        public string? HeaderJSON { get; set; }

        public string ? BodyJSON { get; set; }
        public string ? ParentFolderID { get; set; }

        public string RequestName { get; set; }

    }
}
