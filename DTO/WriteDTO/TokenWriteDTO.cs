using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DTO.WriteDTO
{
    public class TokenWriteDTO
    {
        [Required]
        [MaxLength]
        public string AccessToken { get; set; }
        [Required]
        public int AccessTokenExpriesIn { get; set; }

        [Required]
        [MaxLength]
        public string RefreshToken { get; set; }
        [Required]
        public int RefreshTokenExpriesIn { get; set; }

        [Required]
        public Guid UserID { get; set; }
    }
}
