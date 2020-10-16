using YSharp.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace YSharp.Models
{
    public class UserViewModel
    {
        public UserViewModel() { }
        public UserViewModel(ApplicationUser entity, string role)
        {
            this.Id = entity.Id;
            this.UserName = entity.UserName;
            this.Role = role;
            this.Email = entity.Email;
            this.EmailConfirmed = entity.EmailConfirmed;
            this.PhoneNumber = entity.PhoneNumber;
            this.PhoneNumberConfirmed = entity.PhoneNumberConfirmed;
            this.Name = entity.Name;
            this.CreateDate = entity.CreateDate;
            this.IsSuperAdmin = entity.IsSuperAdmin;
           this.IsLocked = entity.LockoutEnabled && entity.LockoutEnd.HasValue && entity.LockoutEnd.Value > DateTime.Now;
            this.AvatarId = entity.Avatar == null ? "" : entity.Avatar.Id;
            this.AvatarUrl = entity.Avatar == null ? "" : entity.Avatar.AbsolutePath;
        }
        public string Id { set; get; }
        public bool IsLocked { set; get; }

        public string UserName { set; get; }
        public string Password { set; get; }

        [Display(Name = "Role")]
        public string Role { set; get; }

        public string Email { set; get; }
        public bool EmailConfirmed { set; get; }

        public string PhoneNumber { set; get; }
        public bool PhoneNumberConfirmed { set; get; }

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

        public string AvatarId { get; set; }

        public string AvatarUrl { get; set; }
    }
}
