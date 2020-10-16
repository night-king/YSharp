using Microsoft.AspNetCore.Http;
using YSharp.Domain;
using YSharp.Models;
using YSharp.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YSharp.Services
{
    public interface IRoleService
    {
        /// <summary>
        /// 创建Role
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        bool Create(RoleViewModel model, out string errMsg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        RoleViewModel GetById(string id);

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        bool Edit(RoleViewModel model, out string errMsg);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserId"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        bool Delete(string id, string currentUserId, out string errMsg);

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        PagedList<RoleViewModel> Query(string key, int pageIndex, int pageSize);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="permissions"></param>
        void SavePermission(string roleId, IEnumerable<RolePermissionViewModel> permissions);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        IEnumerable<RolePermissionViewModel> GetPermission(string roleId);
    }

    public class RoleService : IRoleService
    {
        readonly IUnitOfWork _uow;
        public RoleService(IUnitOfWork uow)
        {
            this._uow = uow;
        }
        public bool Create(RoleViewModel model, out string errMsg)
        {
            var id = RandomIdGenerator.NewId();
            var entity = new ApplicationRole
            {
                Id = id,
                Name = model.Name,
                CreateDate = DateTime.Now,
                NormalizedName = model.Name.ToUpper(),
            };
            _uow.Set<ApplicationRole>().Add(entity);
            _uow.Commit();
            errMsg = "Success";
            return true;
        }

        public bool Delete(string id, string currentUserId, out string errMsg)
        {
            var entity = _uow.Set<ApplicationRole>().Find(id);
            if (entity == null)
            {
                errMsg = "Can't find this Role";
                return false;
            }
            _uow.Set<ApplicationRole>().Remove(entity);
            _uow.Commit();
            errMsg = "Success";
            return true;
        }

        public bool Edit(RoleViewModel model, out string errMsg)
        {
            var entity = _uow.Set<ApplicationRole>().Find(model.Id);
            if (entity == null)
            {
                errMsg = "Can't find this role";
                return false;
            }
            entity.Name = model.Name;
            _uow.Commit();
            errMsg = "Success";
            return true;
        }

        public RoleViewModel GetById(string id)
        {
            var entity = _uow.Set<ApplicationRole>().Find(id);
            if (entity == null)
            {
                return null;
            }
            return new RoleViewModel(entity);
        }

        public IEnumerable<RolePermissionViewModel> GetPermission(string roleId)
        {
            return _uow.Set<RolePermission>().Where(x => x.Role.Id == roleId).ToList().Select(x => new RolePermissionViewModel(x));

        }

        public PagedList<RoleViewModel> Query(string key, int pageIndex, int pageSize)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            var query = _uow.Set<ApplicationRole>().AsQueryable();
            if (!string.IsNullOrEmpty(key))
            {
                query = query.Where(x => x.Name.Contains(key));
            }
            var count = query.Count();
            var limit = pageSize;
            var offset = (pageIndex - 1) * limit;
            var items = query.OrderByDescending(x => x.CreateDate).Skip(offset).Take(limit).ToList().Select(x => new RoleViewModel(x));
            return new PagedList<RoleViewModel>(items, pageIndex, pageSize, count);
        }

        public void SavePermission(string roleId, IEnumerable<RolePermissionViewModel> permissions)
        {
            var exist = _uow.Set<RolePermission>().Where(x => x.Role.Id == roleId).ToList();
            if (exist != null && exist.Count > 0)
            {
                _uow.Remove(exist);
            }
            var role = _uow.Set<ApplicationRole>().Find(roleId);
            var rolePermissions = new List<RolePermission>();
            foreach (var per in permissions)
            {
                rolePermissions.Add(new RolePermission
                {
                    Id = RandomId.CreateEnhance(),
                    PermissionId = per.PermissionId,
                    CreateDate = DateTime.Now,
                    RelevantURL = per.RelevantURL,
                    Url = per.Url,
                    Role = role
                });
            }
            _uow.Set<RolePermission>().AddRange(rolePermissions);
            _uow.Commit();
        }
    }

}
