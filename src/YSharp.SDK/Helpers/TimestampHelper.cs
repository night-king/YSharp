using System;
using System.Collections.Generic;
using System.Text;

namespace YSharp.SDK.Helpers
{
    public class TimestampHelper
    {
        /// <summary>
        /// Unix时间戳转当地时间
        /// </summary>
        /// <param name="unixTimestamp"></param>
        /// <returns></returns>
        public static DateTime ConvertToLocalDateTime(long unixTimestamp)
        {
            return new DateTime(1970, 1, 1).AddMilliseconds(unixTimestamp);
        }

        /// <summary>
        /// 或取Unix时间时间戳
        /// </summary>
        /// <param name="nowTime"></param>
        /// <returns></returns>
        public static long ToUnixTime(DateTime nowTime)
        {
            DateTime startTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (long)Math.Round((nowTime - startTime).TotalMilliseconds, MidpointRounding.AwayFromZero);
        }
    }
}
