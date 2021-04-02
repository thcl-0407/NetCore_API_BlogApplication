using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    public class PostComment
    {
        [Column(Order = 1)]
        public int PostID { get; set; }
        public Post Posts { get; set; }

        [Column(Order = 2)]
        public Guid CommentID { get; set; }
        public Comment Comments { get; set; }

        [Required]
        public Guid UserID { get; set; }
    }
}
