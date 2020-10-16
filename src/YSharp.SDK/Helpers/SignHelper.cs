using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace YSharp.SDK.Helpers
{
    public class SignHelper
    {
        /// <summary>
        /// 计算签名
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        public static string Sign(Dictionary<string, string> parameters, string secretKey)
        {
            var sb = new StringBuilder();
            foreach (var key in parameters.Keys.OrderBy(x => x))
            {
                var v = parameters[key];
                if (string.IsNullOrEmpty(v))
                {
                    continue;
                }
                sb.Append(key + "=" + v + "&");
            }
            sb.Append("key=" + secretKey);
            var string1 = sb.ToString();
            var sign = MD5(string1).ToUpper();
            return sign;
        }

        /// <summary>
        /// 签名校验
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="signedStr"></param>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        public static bool Verify(Dictionary<string, string> parameters, string signedStr, string secretKey)
        {
            var sign1 = Sign(parameters, secretKey);
            return sign1 == signedStr;
        }
        public static string MD5(string source)
        {
            byte[] sor = Encoding.UTF8.GetBytes(source);
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] result = md5.ComputeHash(sor);
            StringBuilder strbul = new StringBuilder(40);
            for (int i = 0; i < result.Length; i++)
            {
                strbul.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位
            }
            return strbul.ToString();
        }
    }
}
