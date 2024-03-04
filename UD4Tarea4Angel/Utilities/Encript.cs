using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UD4Tarea4Angel.Utilities
{
    public class Encript
    {
        public static string GetSHA256(string input)
        {
            SHA256 sha256 = SHA256.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream;
            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(input));
            for (int i = 0; i < stream.Length; i++)
            {
                sb.AppendFormat("{0:x2}", stream[i]);
            }

            return sb.ToString();
        }
    }
}
