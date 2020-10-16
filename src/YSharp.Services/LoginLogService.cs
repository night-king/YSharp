using YSharp.Domain;
using YSharp.Models;
using YSharp.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YSharp.Services
{
    public interface ILoginLogService
    {

        /// <summary>
        /// 创建CustomerBranding
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        bool Create(LoginLogViewModel model, out string errMsg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        LoginLogViewModel GetById(string id);

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="username"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        PagedList<LoginLogViewModel> Query(int pageIndex, int pageSize, string username = "", string ip = "");
    }
    public class LoginLogService : ILoginLogService
    {
        readonly IUnitOfWork _uow;
        public LoginLogService(IUnitOfWork uow)
        {
            this._uow = uow;
        }
        public bool Create(LoginLogViewModel model, out string errMsg)
        {
            var ip = model.IP;
            var location = "";
            if (!string.IsNullOrEmpty(ip))
            {
                var ipAddress = _uow.Set<IPAddress>().Find(ip);
                if (ipAddress == null)
                {
                    var ipRegion = IP2RegionHelper.Convert(ip);
                    if (ipRegion != null)
                    {
                        ipAddress = new IPAddress
                        {
                            City = ipRegion.city,
                            Country = ipRegion.country,
                            CreateDate = DateTime.Now,
                            Detail = ipRegion.ToString(),
                            IP = model.IP,
                            ElapsedMilliseconds = ipRegion.ElapsedMilliseconds,
                            Province = ipRegion.province,
                            Source = ipRegion.from
                        };
                        _uow.Set<IPAddress>().Add(ipAddress);
                    }
                }
                location = ipAddress == null ? "" : ipAddress.Detail;
            }

            var entity = new LoginLog
            {
                Id = RandomId.CreateEnhance(),
                Description = model.Description,
                IP = model.IP,
                IsSuccess = model.IsSuccess,
                Location = location,
                LoginUrl = model.LoginUrl,
                UserName = model.UserName,
                CreateDate = DateTime.Now,
            };
            _uow.Set<LoginLog>().Add(entity);
            var user = _uow.Set<ApplicationUser>().FirstOrDefault(x => x.UserName == model.UserName);
            var sender = _uow.Set<ApplicationUser>().FirstOrDefault(x => x.IsSuperAdmin == true);
            _uow.Set<Message>().Add(new Message
            {
                Accepter = user,
                CreateDate = DateTime.Now,
                Sender = sender,
                Content = "Login successfully, login ip: " + model.IP + ", If you are not the operator, please change the password immediately.",
                Id = RandomId.CreateEnhance()
            });
            _uow.Commit();
            errMsg = "Success";
            return true;
        }

        public LoginLogViewModel GetById(string id)
        {
            var entity = _uow.Set<LoginLog>().Find(id);
            if (entity == null)
            {
                return null;
            }
            return new LoginLogViewModel(entity);
        }

        public PagedList<LoginLogViewModel> Query(int pageIndex, int pageSize, string username = "", string ip = "")
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            var query = _uow.Set<LoginLog>().AsQueryable();
            if (!string.IsNullOrEmpty(username))
            {
                query = query.Where(x => x.UserName == username);
            }
            if (!string.IsNullOrEmpty(ip))
            {
                query = query.Where(x => x.IP == ip);
            }
            var count = query.Count();
            var limit = pageSize;
            var offset = (pageIndex - 1) * limit;
            var items = query.OrderByDescending(x => x.CreateDate).Skip(offset).Take(limit).ToList().Select(x => new LoginLogViewModel(x));
            return new PagedList<LoginLogViewModel>(items, pageIndex, pageSize, count);

        }
    }
}
