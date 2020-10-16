using System;
using System.Collections.Generic;
using System.Text;

namespace YSharp.Domain
{
    public class AccessLog : BaseEntity<string>
    {
        /// <summary>
        /// 
        /// </summary>
        public string Url { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string AbsolutelyUrl { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string RequestIP { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string RequestContentType { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string RequestMethod { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string RequestBody { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime RequestDate { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string ResponseStatusCode { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string ResponseContentType { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string ResponseBody { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime ResponseDate { set; get; }

        /// <summary>
        /// 耗时（毫秒）
        /// </summary>
        public int SpendMilliseconds { set; get; }


    }
}
