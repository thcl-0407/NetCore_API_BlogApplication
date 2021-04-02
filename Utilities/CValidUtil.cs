using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Utilities
{
    public static class CValidUtil
    {
        private static readonly string REGEX_PATTERN_EMAIL = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
            
        //Check A Email Is Valid
        public static bool isValidEmail(string email) {

            Regex regex = new Regex(REGEX_PATTERN_EMAIL);

            return regex.IsMatch(email);
        }

        //Check A Password Is Valid
        public static bool isValidPassword(string password)
        {
            if(password.Length < 6 || password.Length > 26)
            {
                return false;
            }

            return true;
        }
    }
}
