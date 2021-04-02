using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DTO.WriteDTO
{
    public class UserWriteDTO
    {
        public Guid UserID { get; set; }

        public string UserName { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Mật Khẩu Dài Từ 6 Đến 16 Kí Tự")]
        [MaxLength(16, ErrorMessage = "Mật Khẩu Dài Từ 6 Đến 16 Kí Tự")]
        public string Password { get; set; }

        [Required]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")]
        public string Email { get; set; }

        public bool isAuthenticated { get; set; }


        public bool isValueNull ()
        {
            if(this.UserID == null)
            {
                return true;
            }

            if (this.UserName == null)
            {
                return true;
            }

            if (this.Email == null)
            {
                return true;
            }

            if (this.Password == null)
            {
                return true;
            }

            return false;
        }
    }
}
