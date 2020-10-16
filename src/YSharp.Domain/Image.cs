using System;
using System.Collections.Generic;
using System.Text;

namespace YSharp.Domain
{
    /// <summary>
    /// 图片
    /// </summary>
    public class Image : FakeDeleteEntity<string>
    {

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

    }
}
