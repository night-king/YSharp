using YSharp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace YSharp.Models
{
    public class RolePermissionViewModel
    {
        public RolePermissionViewModel()
        {

        }
        public RolePermissionViewModel(RolePermission entity)
        {
            this.Id = entity.Id;
            this.PermissionId = entity.PermissionId;
            this.Url = entity.Url;
            this.RelevantURL = entity.RelevantURL;
        }

        public string Id { get; set; }

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
