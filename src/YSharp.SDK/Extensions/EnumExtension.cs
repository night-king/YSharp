using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YSharp.SDK
{
    /// <summary>
    /// 枚举扩展
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 获取描述文字
        /// </summary>
        /// <param name="enumSubitem"></param>
        /// <returns></returns>
        public static string ToDescription(this Enum enumSubitem)
        {
            string strValue = enumSubitem.ToString();

            FieldInfo fieldinfo = enumSubitem.GetType().GetField(strValue);

            if (fieldinfo == null)
            {
                return "";
            }

            Object[] objs = fieldinfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (objs == null || objs.Length == 0)
            {
                return strValue;
            }
            else
            {
                DescriptionAttribute da = (DescriptionAttribute)objs[0];
                return da.Description;
            }
        }
    }
}

