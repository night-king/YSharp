using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YSharp.SDK
{
    public static class ListExtention
    {
        public static string ToStringSplit(this IEnumerable<string> source, string split = ",")
        {
            if (source == null || source.Count() == 0)
            {
                return string.Empty;
            }
            var count = source.Count();
            var sb = new StringBuilder();
            for (var i = 0; i < count; i++)
            {
                sb.Append(source.ElementAt(i));
                if (i < count - 1)
                {
                    sb.Append(split);
                }
            }
            return sb.ToString();
        }
    }
}
