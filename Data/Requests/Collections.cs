using Skipperu.Data.Users.data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skipperu.Data.Requests
{
    public class Collection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string FolderRootID { get; set; }
        public string FolderNameNormalized { get; set; }

        public string FolderPathNormalized { get; set; }
        public string? GlobalUserID { get; set; }

        [NotMapped]
        public GlobalUser? UserNav { get; set; }
        public string? ParentFolderID { get; set; } //TODO: Change to CollectionParentFolderID

        public virtual List<RequestDBstore> SavedRequests { get; set; }

    }
}
