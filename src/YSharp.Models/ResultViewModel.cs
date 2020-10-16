using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YSharp.Models
{
    public class ResultViewModel
    {
        public bool Succeeded { set; get; }

        public string Title { set; get; }

        public string Message { set; get; }

        public string ReturnUrl { set; get; }

        public string Style
        {
            set; get;
        }

    }
}
