using System;
using System.Collections.Generic;
using System.Text;

namespace YSharp.Domain
{
    /// <summary>
    /// 登录日志
    /// </summary>
    public class LoginLog : BaseEntity<string>
    {
        /// <summary>
        /// 登录用户名
        /// </summary>
        public string UserName { set; get; }

        /// <summary>
        /// 登录地址
        /// </summary>
        public string LoginUrl { set; get; }

        /// <summary>
        /// IP
        /// </summary>
        public string IP { set; get; }

        /// <summary>
        /// 登录所在地
        /// </summary>
        public string Location { set; get; }

        /// <summary>
        /// 登录是否成功
        /// </summary>
        public bool IsSuccess { set; get; }

        /// <summary>
        /// 登录描述
        /// </summary>
        public string Description { set; get; }

    }
}
