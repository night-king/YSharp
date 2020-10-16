using YSharp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace YSharp.Models
{
    public class ImageViewModel
    {
        public ImageViewModel() { }
        public ImageViewModel(Image entity)
        {
            this.Id = entity.Id;
            this.FileName = entity.FileName;
            this.Size = entity.Size;
            this.VirtualPath = entity.VirtualPath;
            this.AbsolutePath = entity.AbsolutePath;
            this.CreateDate = entity.CreateDate;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Id { set; get; }

        /// <summary>
        /// 名称
        /// </summary>
        public string FileName { set; get; }

        /// <summary>
        /// 大小
        /// </summary>
        public long Size { set; get; }

        /// <summary>
        /// 相对地址
        /// </summary>
        public string VirtualPath { set; get; }

        /// <summary>
        /// 绝对地址
        /// </summary>
        public string AbsolutePath { set; get; }
        /// <summary>
        /// 
        /// </summary>

        public DateTime CreateDate { set; get; }

    }
}
