using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace DTO.WriteDTO
{
    public class CommentWriteDTO
    {
        [Required]
        public Guid CommentID { get; set; }

        [Required]
        public string CommentContent { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public Guid UserID { get; set; }

        [Required]
        public int PostID { get; set; }
    }
}
