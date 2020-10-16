using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace YSharp.Models
{
    public class SetPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "NewPassword")]
        public string NewPassword
        {
            get; set;
        }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "You enter two passwords do not match")]
        public string ConfirmPassword
        {
            get; set;
        }

    }
}
