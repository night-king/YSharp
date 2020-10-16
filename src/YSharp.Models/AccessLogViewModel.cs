using YSharp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace YSharp.Models
{
    public class AccessLogViewModel
    {
        public AccessLogViewModel()
        {

        }
        public AccessLogViewModel(AccessLog entity)
        {
            this.Id = entity.Id;
            this.Url = entity.Url;
            this.AbsolutelyUrl = entity.AbsolutelyUrl;
            this.RequestIP = entity.RequestIP;
            this.RequestMethod = entity.RequestMethod;
            this.RequestContentType = entity.RequestContentType;
            this.RequestBody = entity.RequestBody;
            this.RequestDate = entity.RequestDate;
            this.ResponseStatusCode = entity.ResponseStatusCode;
            this.ResponseContentType = entity.ResponseContentType;
            this.ResponseBody = entity.ResponseBody;
            this.ResponseDate = entity.ResponseDate;
            this.SpendMilliseconds = entity.SpendMilliseconds;
            this.CreateDate = entity.CreateDate;
        }
        public string Id { set; get; }

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
        public string RequestMethod { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string RequestContentType { set; get; }

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

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { set; get; }

    }
}
