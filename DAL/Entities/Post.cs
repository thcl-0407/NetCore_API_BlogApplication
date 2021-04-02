using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    [Table("Post")]
    public class Post
    {
        [Key]
        public int PostID { get; set; }

        [Required]
        [MaxLength]
        public string TitlePost { get; set; }

        [Required]
        [MaxLength]
        public string SummaryPost { get; set; }

        [Required]
        [MaxLength]
        public string ContentPost { get; set; }

        [Required]
        public DateTime DateCreate { get; set; }

        public DateTime DateUpdate { get; set; }

        [Required]
        public Guid ImageID { get; set; }
        public ImageGallery ImageGallery { get; set; }

        [Required]
        public Guid  UserID { get; set; }
        public User User { get; set; }

        public ICollection<PostComment> PostComments { get; set; }
    }
}
