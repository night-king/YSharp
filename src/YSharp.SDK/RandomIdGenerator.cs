using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace YSharp.SDK
{
    public class RandomIdGenerator
    {
        /// <summary>
        /// 生成Id
        /// </summary>
        /// <returns></returns>
        public static string NewId()
        {
            byte[] randomBytes = new byte[4];
            RNGCryptoServiceProvider rngCrypto = new RNGCryptoServiceProvider();
            rngCrypto.GetBytes(randomBytes);
            return DateTime.Now.ToString("yyyyMMdd") + Math.Abs(BitConverter.ToInt32(randomBytes, 0)).Uniform(10);
        }

    }
}