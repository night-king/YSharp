using YSharp.Domain;
using YSharp.Models;
using YSharp.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YSharp.Services
{
    public interface IAccessLogService
    {

        /// <summary>
        /// 创建CustomerBranding
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        bool Create(AccessLogViewModel model, out string errMsg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AccessLogViewModel GetById(string id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        PagedList<AccessLogViewModel> Query(string key, string start, string end, int pageIndex, int pageSize, string ip = "");
    }
    public class AccessLogService : IAccessLogService
    {
        readonly IUnitOfWork _uow;
        public AccessLogService(IUnitOfWork uow)
        {
            this._uow = uow;
        }
        public bool Create(AccessLogViewModel model, out string errMsg)
        {
            var entity = new AccessLog
            {
                Id = model.Id,
                Url = model.Url,
                AbsolutelyUrl = model.AbsolutelyUrl,
                RequestIP = model.RequestIP,
                RequestContentType = model.RequestContentType,
                RequestBody = model.RequestBody,
                RequestDate = model.RequestDate,
                ResponseStatusCode = model.ResponseStatusCode,
                ResponseContentType = model.ResponseContentType,
                ResponseBody = model.ResponseBody,
                ResponseDate = model.ResponseDate,
                SpendMilliseconds = model.SpendMilliseconds,
                CreateDate = model.CreateDate
            };
            _uow.Set<AccessLog>().Add(entity);
            _uow.Commit();
            errMsg = "Success";
            return true;
        }

        public AccessLogViewModel GetById(string id)
        {
            var entity = _uow.Set<AccessLog>().Find(id);
            if (entity == null)
            {
                return null;
            }
            return new AccessLogViewModel(entity);
        }

        public PagedList<AccessLogViewModel> Query(string key, string start, string end, int pageIndex, int pageSize, string ip = "")
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            var query = _uow.Set<AccessLog>().AsQueryable();
            if (!string.IsNullOrEmpty(key))
            {
                query = query.Where(x => x.Url.Contains(key) || x.AbsolutelyUrl.Contains(key));
            }
            if (!string.IsNullOrEmpty(ip))
            {
                query = query.Where(x => x.RequestIP == ip);
            }
            var startDate = start.ToDateTime();
            var endDate = end.ToDateTime();
            if (startDate.HasValue)
            {
                query = query.Where(x => x.CreateDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(x => x.CreateDate <= endDate.Value);
            }
            var count = query.Count();
            var limit = pageSize;
            var offset = (pageIndex - 1) * limit;
            var items = query.OrderByDescending(x => x.CreateDate).Skip(offset).Take(limit).ToList().Select(x => new AccessLogViewModel(x));
            return new PagedList<AccessLogViewModel>(items, pageIndex, pageSize, count);

        }
    }
}
