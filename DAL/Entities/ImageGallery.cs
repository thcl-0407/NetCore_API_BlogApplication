using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Entities
{
    public class ImageGallery
    {
        [Key]
        public Guid ImageID { get; set; }

        [MaxLength]
        public string Base64Code { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}
