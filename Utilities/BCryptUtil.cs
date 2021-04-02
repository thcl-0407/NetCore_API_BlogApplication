using System;

namespace Utilities
{
    public static class BCryptUtil
    { 
        public static string HashPassword(string None_HashPassword)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(None_HashPassword, hashType:BCrypt.Net.HashType.SHA256);
        }

        public static bool VerifyPassword(string InputPassword, string HashPassword)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(InputPassword, HashPassword, hashType:BCrypt.Net.HashType.SHA256);
        }
    }
}
