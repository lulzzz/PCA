using System.Security.Cryptography;
using System.Text;

namespace PrivateCert.LibCore.Infrastructure
{
    public static class StringHash
    {
        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = SHA512.Create();  
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(byte[] bytes)
        {
            // Convert byte array to a string   
            StringBuilder builder = new StringBuilder();  
            for (int i = 0; i < bytes.Length; i++)  
            {  
                builder.Append(bytes[i].ToString("x2"));  
            }  
            return builder.ToString(); 
        }
    }
}
