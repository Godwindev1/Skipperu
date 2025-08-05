using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Skipperu.Data.Requests;
using Skipperu.Data.Users.data.ExternalAuth.data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skipperu.Data.Users.data
{
    public class GlobalUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string GlobalUserID { get; set; }
        public string? AspFK { get; set; }     
        public string? GoogleAuthFK { get; set; }

        public string NormalizedUserEmail { get; set; }

        public IdentityUser? AspUser { get; set; }
        public GoogleUser? GoogleAuth { get; set; }

        public bool IsAspIdentityUser => AspFK != null;
        public bool IsExternalAuthUser => GoogleAuthFK != null;


    }

}
