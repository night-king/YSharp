using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace YSharp.Domain
{

    /// <summary>
    /// 用户消息
    /// </summary>
    public class Message : BaseEntity<string>
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual ApplicationUser Sender { set; get; }

        /// <summary>
        /// 接收者
        /// </summary>
        public virtual ApplicationUser Accepter { set; get; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }

    }
}
