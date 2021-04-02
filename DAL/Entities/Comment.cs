using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    [Table("Comment")]
    public class Comment
    {
        [Key]
        public Guid CommentID { get; set; }

        [MaxLength]
        public string ContentComment { get; set; }

        [Required]
        public DateTime DateCreate { get; set; }

        public ICollection<PostComment> PostComments { get; set; }
    }
}
