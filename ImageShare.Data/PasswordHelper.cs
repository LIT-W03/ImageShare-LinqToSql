using System;
using System.Security.Cryptography;
using System.Text;

namespace ImageShare.Data
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password, string salt)
        {
            SHA256Managed crypt = new SHA256Managed();

            string combinedString = password + salt;
            byte[] combined = Encoding.Unicode.GetBytes(combinedString);

            byte[] hash = crypt.ComputeHash(combined);
            return Convert.ToBase64String(hash);
        }

        public static string GenerateSalt()
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[10];
            provider.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public static bool IsMatch(string passwordToCheck, string hashedPassword, string salt)
        {
            string hash = HashPassword(passwordToCheck, salt);
            return hash == hashedPassword;
        }
    }
}