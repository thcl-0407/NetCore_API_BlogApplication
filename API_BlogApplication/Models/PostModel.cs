using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API_BlogApplication.Models
{
    public class PostModel
    {
        [Required]
        [MaxLength]
        public string Title { get; set; }

        [Required]
        [MaxLength]
        public string Summary { get; set; }

        [Required]
        [MaxLength]
        public string Content { get; set; }

        [Required]
        [MaxLength]
        public string EncodeImage { get; set; }
    }
}
