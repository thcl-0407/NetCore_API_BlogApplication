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

        public bool isPropertiesNull()
        {
            if (this.TitlePost == null)
            {
                return true;
            }

            if (this.SummaryPost == null)
            {
                return true;
            }

            if (this.TitlePost == null)
            {
                return true;
            }

            if (this.ContentPost == null)
            {
                return true;
            }

            if (this.EncodeImage == null)
            {
                return true;
            }

            if (this.UserID == null)
            {
                return true;
            }

            return false;
        }

        public bool isPropertiesNullWithoutEndcode()
        {
            if (this.TitlePost == null)
            {
                return true;
            }

            if (this.SummaryPost == null)
            {
                return true;
            }

            if (this.TitlePost == null)
            {
                return true;
            }

            if (this.ContentPost == null)
            {
                return true;
            }

            if (this.UserID == null)
            {
                return true;
            }

            return false;
        }

        public bool isPropertiesEmpty()
        {
            if (this.TitlePost.Trim().Length == 0)
            {
                return true;
            }

            if (this.SummaryPost.Trim().Length == 0)
            {
                return true;
            }

            if (this.TitlePost.Trim().Length == 0)
            {
                return true;
            }

            if (this.ContentPost.Trim().Length == 0)
            {
                return true;
            }

            if (this.EncodeImage.Trim().Length == 0)
            {
                return true;
            }

            if (this.UserID.Trim().Length == 0)
            {
                return true;
            }

            return false;
        }

        public bool isPropertiesEmptyWithoutEndcode()
        {
            if (this.TitlePost.Trim().Length == 0)
            {
                return true;
            }

            if (this.SummaryPost.Trim().Length == 0)
            {
                return true;
            }

            if (this.TitlePost.Trim().Length == 0)
            {
                return true;
            }

            if (this.ContentPost.Trim().Length == 0)
            {
                return true;
            }

            if (this.UserID.Trim().Length == 0)
            {
                return true;
            }

            return false;
        }
    }
}
