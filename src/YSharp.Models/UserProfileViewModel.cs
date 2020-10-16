using YSharp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace YSharp.Models
{
    public class UserProfileViewModel
    {
        public UserViewModel User { set; get; }

        public IEnumerable<LoginLogViewModel> LoginLogs { set; get; }


    }
}
