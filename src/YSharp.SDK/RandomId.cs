using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YSharp.SDK
{
    public enum RandomIdStyleEnum
    {
        /// <summary>
        /// 纯数字
        /// </summary>
        Number = 1,

        /// <summary>
        /// 英文字母
        /// </summary>
        Letter = 2,

        /// <summary>
        /// 字母数字组合
        /// </summary>
        Mix = 3,
    }

    public class RandomId
    {
        private static string[] _code_mixs = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        private static string[] _code_letters = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        private static string[] _code_numbers = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
        /// <summary>
        /// 为了生成更加可靠的随机数，微软在System.Security.Cryptography命名空间下提供一个名为system.Security.Cryptography.RNGCryptoServiceProvider的类，
        /// 它采用系统当前的硬件信息、进程信息、线程信息、系统启动时间和当前精确时间作为填充因子，通过更好的算法生成高质量的随机数
        /// 该类初始化会比较消耗内存，故改为static
        /// </summary>
        private static RNGCryptoServiceProvider rngProvider = new RNGCryptoServiceProvider();

        /// <summary>
        /// 获取随机字符串（轻量版）
        /// </summary>
        /// <param name="length">字符串长度</param>
        /// <param name="style">字符串样式</param>
        /// <returns></returns>
        public static string Create(int length, RandomIdStyleEnum style = RandomIdStyleEnum.Number)
        {
            if (length <= 0) { return string.Empty; }
            var sb = new StringBuilder();
            string[] code = null;
            switch (style)
            {
                case RandomIdStyleEnum.Letter:
                    code = _code_letters;
                    break;
                case RandomIdStyleEnum.Number:
                    code = _code_numbers;
                    break;
                case RandomIdStyleEnum.Mix:
                    code = _code_mixs;
                    break;
            }
            var cl = code.Length - 1;
            var r = new Random();
            for (var i = 0; i < length; i++)
            {
                var n = r.Next(cl);
                sb.Append(code[n]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取随机字符串（纯数字）,固定10位，绝不重复，原理类似Guid.
        /// </summary>
        /// <returns></returns>
        public static string CreateEnhance()
        {
            byte[] randomBytes = new byte[4];
            rngProvider.GetBytes(randomBytes);
            return Math.Abs(BitConverter.ToInt32(randomBytes, 0)).Uniform(10);

        }
    }
}
