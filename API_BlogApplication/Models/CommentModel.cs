using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace API_BlogApplication.Models
{
    public class CommentModel
    {
        [Required]
        public string CommentContent { get; set; }

        [Required]
        public int PostID { get; set; }
    }
}
