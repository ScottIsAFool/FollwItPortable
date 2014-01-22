using System.Security.Cryptography;
using System.Text;

namespace FollwItPortable
{
    internal static class Utils
    {
        internal static string Hash(this string password)
        {
            // salt + hash
            string salt = "52c3a0d0-f793-46fb-a4c0-35a0ff6844c8";
            string saltedPassword = password + salt;
            string sHash = "";

            var sha1Obj = new SHA1Managed();
            byte[] bHash = sha1Obj.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));

            foreach (byte b in bHash)
            {
                sHash += b.ToString("x2");
            }

            return sHash;
        }
    }
}
