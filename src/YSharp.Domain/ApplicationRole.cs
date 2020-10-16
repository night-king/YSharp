using Microsoft.AspNetCore.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YSharp.Domain
{
    public class ApplicationRole : IdentityRole
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        public virtual ICollection<RolePermission> RolePermissions { set; get; }

        public ApplicationRole()
        {
            CreateDate = DateTime.Now;
        }

    }
}
