using YSharp.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace YSharp.Models
{
    public class RoleViewModel
    {
        public RoleViewModel() { }
        public RoleViewModel(ApplicationRole entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            CreateDate = entity.CreateDate;
        }

        public string Id { set; get; }

        [Required]
        public string Name { set; get; }


        public DateTime CreateDate { get; set; }

    }
}
