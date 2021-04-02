using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DTO.WriteDTO
{
    public class PostWriteDTO
    {
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
        public DateTime DateCreated { get; set; }

        public string ImageID { get; set; }

        [Required]
        [MaxLength]
        public string EncodeImage { get; set; }

        [Required]
        public string UserID { get; set; }
    }
}
