using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.ReadDTO
{
    public class CommentReadDTO
    {
        public Guid CommentID { get; set; }
        public string CommentContent { get; set; }
        public DateTime DateCreated { get; set; }
        public Guid UserID { get; set; }
        public int PostID { get; set; }
        public string UserName { get; set; }
    }
}
