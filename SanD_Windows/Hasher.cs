using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SanD_Windows
{
    class Hasher
    {
        public static string GenerateFileHash(string FilePath)
        {
            using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            {
                using (SHA512Managed SHA = new SHA512Managed())
                {
                    byte[] hash = SHA.ComputeHash(bs);
                    StringBuilder formatted = new StringBuilder(2 * hash.Length);
                    foreach (byte b in hash)
                    {
                        formatted.AppendFormat("{0:X2}", b);
                    }
                    return formatted.ToString();
                }
            }
        }
    }

}