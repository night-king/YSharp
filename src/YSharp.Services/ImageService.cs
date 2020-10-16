using Aliyun.OSS;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using YSharp.Domain;
using YSharp.Models;
using YSharp.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace YSharp.Services
{
    public interface IImageService
    {
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="fileName">存储的文件名</param>
        /// <param name="buffer"></param>
        /// <param name="folder">子目录</param>
        /// <returns>文件券路径</returns>
        ImageViewModel Create(string fileName, byte[] buffer, string folder);

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="fileName">存储的文件名</param>
        /// <param name="formFile"></param>
        /// <param name="folder">子目录</param>
        /// <returns>文件券路径</returns>
        ImageViewModel Create(string fileName, IFormFile formFile, string folder);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ImageViewModel GetById(string id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserId"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        bool Delete(string id, string currentUserId, out string errMsg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        PagedList<ImageViewModel> Query(int pageIndex, int pageSize);
    }
    public class ImageService : IImageService
    {
        readonly IUnitOfWork _uow;
        private IConfiguration _config;

        public ImageService(IUnitOfWork uow, IConfiguration config)
        {
            this._uow = uow;
            this._config = config;
        }

        public ImageViewModel Create(string fileName, byte[] buffer, string folder)
        {
            var ossClient = new OssClient(_config.GetSection("AliyunOSS")["Endpoint"], _config.GetSection("AliyunOSS")["AccessKey"],
                                          _config.GetSection("AliyunOSS")["AccessSecret"]);

            var bucketName = _config.GetSection("AliyunOSS")["BucketName"];
            var domain = _config.GetSection("AliyunOSS")["Domain"];
            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
            {
                extension = ".jpg";
            }
            var contentType = "";
            switch (extension.ToLower())
            {
                case ".jpg":
                    contentType = "image/jpg";
                    break;
                case ".jpeg":
                    contentType = "image/jpeg";
                    break;
                case ".png":
                    contentType = "image/png";
                    break;
                case ".gif":
                    contentType = "image/gif";
                    break;
                case ".bmp":
                    contentType = "image/bmp";
                    break;
                case ".tif":
                    contentType = "image/tiff";
                    break;
                default:
                    contentType = "image/jpeg";
                    break;
            }
            var id = RandomId.CreateEnhance();
            var virtualPath = string.Format("{0}/{1}", folder, id + extension);
            var absolutePath = string.Format("{0}/{1}", domain, virtualPath);
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                try
                {
                    var ret = ossClient.PutObject(bucketName, virtualPath, ms, new ObjectMetadata() { ContentType = contentType });

                    if (ret.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var img = new Image
                        {
                            CreateDate = DateTime.Now,
                            VirtualPath = virtualPath,
                            FileName = fileName,
                            Size = (int)ms.Length,
                            Id = id,
                            AbsolutePath = absolutePath,
                        };
                        _uow.Set<Image>().Add(img);
                        _uow.Commit();

                        return new ImageViewModel(img);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }

            }
            return null;
        }

        public ImageViewModel Create(string fileName, IFormFile formFile, string folder)
        {
            using (var sm = formFile.OpenReadStream())
            {
                var length = (int)sm.Length;
                var bytes = new byte[length];
                sm.Read(bytes, 0, length);
                return Create(fileName, bytes, folder);
            }
        }

        public bool Delete(string id, string currentUserId, out string errMsg)
        {
            try
            {

                var ossClient = new OssClient(_config.GetSection("AliyunOSS")["Endpoint"], _config.GetSection("AliyunOSS")["AccessKey"],
                                            _config.GetSection("AliyunOSS")["AccessSecret"]);
                var bucketName = _config.GetSection("AliyunOSS")["BucketName"];
                var img = _uow.Set<Image>().Find(id);

                var virtualPath = img.VirtualPath;
                ossClient.DeleteObject(bucketName, virtualPath);
                img.IsDeleted = true;
                img.DeleteBy = currentUserId;
                img.DeleteDate = DateTime.Now;
                _uow.Commit();
                errMsg = "Success";
                return true;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return false;
            }
        }


        public ImageViewModel GetById(string id)
        {
            var img = _uow.Set<Image>().Find(id);
            if (img == null)
            {
                return null;
            }
            return new ImageViewModel(img);
        }

        public PagedList<ImageViewModel> Query(int pageIndex, int pageSize)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            var query = _uow.Set<Image>().AsQueryable();
            var count = query.Count();
            var limit = pageSize;
            var offset = (pageIndex - 1) * limit;
            var items = query.OrderByDescending(x => x.CreateDate).Skip(offset).Take(limit).ToList().Select(x => new ImageViewModel(x));
            return new PagedList<ImageViewModel>(items, pageIndex, pageSize, count);
        }

    }
}
