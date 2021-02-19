using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ControleDeEstoque.Web.Helpers
{
    public static class CriptoHelper
    {
        public static string HashMD5(string val)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(val);
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(bytes);

            string ret = string.Empty;            
            for (int i = 0; i < hash.Length; i++)
            {
                ret += hash[i].ToString("x2");
            }

            return ret;
        }
    }
}