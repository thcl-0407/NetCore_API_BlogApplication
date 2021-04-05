using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.ReadDTO
{
    public class TokenReadDTO
    {
        public string AccessToken { get; set; }
        public int AccessTokenExpriesIn { get; set; }
        public string RefreshToken { get; set; }
        public int RefreshTokenExpriesIn { get; set; }

        public bool isNullValue()
        {
            if(this.AccessToken == null)
            {
                return true;
            }
            if (this.AccessTokenExpriesIn == null)
            {
                return true;
            }
            if (this.RefreshToken == null)
            {
                return true;
            }
            if (this.RefreshTokenExpriesIn == null)
            {
                return true;
            }

            return false;
        }
    }
}
