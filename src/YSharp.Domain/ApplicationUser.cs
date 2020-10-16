using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace YSharp.Domain
{

    /// <summary>
    /// 用户
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }

        /// <summary>
        /// 是否时超级管理员
        /// </summary>
        public bool IsSuperAdmin { get; set; }


        public bool IsDeleted { set; get; }

        public DateTime? DeleteDate { set; get; }

        public virtual Image Avatar { set; get; }

        public string DeleteBy { set; get; }

        public ApplicationUser()
        {
            CreateDate = DateTime.Now;
        }

    }
}
