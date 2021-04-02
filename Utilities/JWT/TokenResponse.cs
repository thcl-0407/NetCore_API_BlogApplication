using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.JWT
{
    public class TokenResponse
    {
        public string AccessToken { get; set; }
        public int AccessTokenExpiresIn { get; set; }
        public string RefreshToken { get; set; }
        public int RefreshTokenExpiresIn { get; set; }
    }
}
