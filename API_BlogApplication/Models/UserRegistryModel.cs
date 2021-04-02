using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API_BlogApplication.Models
{
    public class UserRegistryModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(26)]
        public string Password { get; set; }
    }
}
