using System;
using System.Collections.Generic;
using System.Text;

namespace YSharp.SDK
{
    public class RandomCodeGenerator
    {
        private static string[] _codes = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        private static string[] _int_codes = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
        public static string NewCode(int length)
        {
            if (length <= 0) { return string.Empty; }
            var r = new Random();
            var sb = new StringBuilder();
            var cl = _codes.Length - 1;
            for (var i = 0; i < length; i++)
            {
                var n = r.Next(cl);
                sb.Append(_codes[n]);
            }
            return sb.ToString();
        }
        public static string NewIntCode(int length)
        {
            if (length <= 0) { return string.Empty; }
            var r = new Random();
            var sb = new StringBuilder();
            var cl = _int_codes.Length - 1;
            for (var i = 0; i < length; i++)
            {
                var n = r.Next(cl);
                sb.Append(_int_codes[n]);
            }
            return sb.ToString();
        }
    }
}
