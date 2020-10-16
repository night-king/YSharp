using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YSharp.Domain
{
    public class RolePermission : BaseEntity<string>
    {
        public virtual ApplicationRole Role
        {
            set; get;
        }

        /// <summary>
        /// 授权ID
        /// </summary>
        public string PermissionId { set; get; }

        /// <summary>
        /// 授权URL
        /// </summary>
        public string Url { set; get; }

        /// <summary>
        /// 授权附带URL
        /// </summary>
        public string RelevantURL { set; get; }

    }
}
