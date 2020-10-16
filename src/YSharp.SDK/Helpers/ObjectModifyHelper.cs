using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YSharp.SDK.Helpers
{
    /// <summary>
    /// 比较两个对象属性差异
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectModifyHelper<T>
    {
        public Dictionary<string, KeyValuePair<string, string>> Compare(T before, T after)
        {
            var dict = new Dictionary<string, KeyValuePair<string, string>>();
            var beforePs = before.GetType().GetProperties();
            var afterPs = after.GetType().GetProperties();
            foreach (var bp in beforePs)
            {
                var propertyName = bp.Name;
                var beforeValue = bp.GetValue(before) == null ? "" : bp.GetValue(before).ToString();
                var ap = afterPs.Where(x => x.Name == propertyName).FirstOrDefault();
                if (ap == null) { continue; }
                var afterValue = ap.GetValue(after) == null ? "" : ap.GetValue(after).ToString();
                if (afterValue != beforeValue)
                {
                    dict.Add(propertyName, new KeyValuePair<string, string>(beforeValue, afterValue));
                }
            }
            return dict;
        }

        public string CompareToString(T before, T after, string newline)
        {
            var dict = Compare(before, after);

            var sb = new StringBuilder();
            var index = 1;
            foreach (var pn in dict.Keys)
            {
                var vs = dict[pn];
                sb.Append("(" + index + ")").Append(pn).Append(":").Append(vs.Key).Append("=>").Append(vs.Value).Append(newline);
                index++;
            }
            return sb.ToString();
        }
    }
}
