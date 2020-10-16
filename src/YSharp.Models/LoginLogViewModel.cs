using YSharp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace YSharp.Models
{
    public class LoginLogViewModel
    {
        public LoginLogViewModel()
        {

        }

        public LoginLogViewModel(LoginLog entity)
        {
            this.Id = entity.Id;
            this.UserName = entity.UserName;
            this.LoginUrl = entity.LoginUrl;
            this.IP = entity.IP;
            this.Location = entity.Location;
            this.IsSuccess = entity.IsSuccess;
            this.Description = entity.Description;
            this.CreateDate = entity.CreateDate;
        }
        public string Id { set; get; }

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

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { set; get;}
    }
}
