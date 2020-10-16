using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace YSharp.Domain
{
    public class IPAddress
    {
        [Key]
        public string IP { set; get; }

        public string Country { set; get; }

        public string Province { set; get; }

        public string City { set; get; }

        /// <summary>
        /// 地点
        /// </summary>
        public string Detail { set; get; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Source { set; get; }

        /// <summary>
        /// 采集花费时间（毫秒）
        /// </summary>
        public int ElapsedMilliseconds { set; get; }

        /// <summary>
        /// 插入时间
        /// </summary>
        public DateTime CreateDate { set; get; }
    }
}
