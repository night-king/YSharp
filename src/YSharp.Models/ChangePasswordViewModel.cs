using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace YSharp.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "当前密码")]
        public string OldPassword
        {
            get; set;
        }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword
        {
            get; set;
        }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("NewPassword", ErrorMessage = "两次输入的密码不一致")]
        public string ConfirmPassword
        {
            get; set;
        }

    }
}
