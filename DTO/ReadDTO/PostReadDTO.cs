using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DTO.ReadDTO
{
    public class PostReadDTO
    {
        public int PostID { get; set; }

        public string TitlePost { get; set; }

        public string SummaryPost { get; set; }

        public string ContentPost { get; set; }

        public DateTime DateCreated { get; set; }

        public string ImageID { get; set; }

        [MaxLength]
        public string EncodeImage { get; set; }

        public string UserID { get; set; }

        public string UserName { get; set; }
    }
}
