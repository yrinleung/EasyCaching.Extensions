using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace EasyCaching.Bus.CAP
{
    /// <summary>
    /// string helper
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// MD5
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToMd5(this string input)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "");
            }
        }
    }
}
