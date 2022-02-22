using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityBasicDotNetCore.Models
{
    public class MFAViewModel
    {
        public string Token { get; set; }

        [Required]
        public string Code { get; set; }
    }
}
