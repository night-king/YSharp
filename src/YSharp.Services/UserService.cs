using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using YSharp.Domain;
using YSharp.Models;
using YSharp.SDK;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace YSharp.Services
{
    public interface IUserService
    {

        /// <summary>
        /// 创建Role
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        bool Create(UserViewModel model, out string errMsg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        UserViewModel GetById(string id);

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        bool Edit(UserViewModel model, out string errMsg);

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
        /// <param name="currenctUserId"></param>
        /// <param name="key"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        PagedList<UserViewModel> Query(string key, int pageIndex, int pageSize);

        /// <summary>
        /// 锁定账号
        /// </summary>
        /// <param name="id"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        bool Lock(string id, out string errMsg);

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newPassword"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        bool ResetPassword(string id, string newPassword, out string errMsg);

        /// <summary>
        /// 解锁账号
        /// </summary>
        /// <param name="id"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        bool UnLock(string id, out string errMsg);

        UserProfileViewModel GetProfile(string id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="subFolderName"></param>
        /// <param name="file"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        bool UploadAvatar(string id, string subFolderName, IFormFile file, out string errMsg);

    }

    public class UserService : IUserService
    {
        readonly IUnitOfWork _uow;
        readonly UserManager<ApplicationUser> _userManager;
        readonly IImageService _imageService;
        public UserService(IUnitOfWork uow,
            UserManager<ApplicationUser> userManager,
            IImageService imageService)
        {
            this._uow = uow;
            this._userManager = userManager;
            this._imageService = imageService;
        }
        public bool Create(UserViewModel model, out string errMsg)
        {
            if (_userManager.FindByNameAsync(model.UserName).Result != null)
            {
                errMsg = "Username is conflict";
                return false;
            }
            var id = RandomIdGenerator.NewId();
            var user = new ApplicationUser
            {
                Id = id,
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = !string.IsNullOrEmpty(model.Email) || model.EmailConfirmed,
                IsSuperAdmin = model.IsSuperAdmin,
                Name = model.Name,
                CreateDate = DateTime.Now,
                PhoneNumber = model.PhoneNumber,
                PhoneNumberConfirmed = !string.IsNullOrEmpty(model.PhoneNumber) || model.PhoneNumberConfirmed,
            };
            var result = _userManager.CreateAsync(user, model.Password).Result;
            if (!result.Succeeded)
            {
                errMsg = string.Join(", ", result.Errors);
                return false;
            }
            if (!string.IsNullOrEmpty(model.Role))
            {
                var result2 = _userManager.AddToRoleAsync(user, model.Role).Result;
                if (!result2.Succeeded)
                {
                    errMsg = string.Join(", ", result2.Errors);
                    return true;
                }
            }
            errMsg = "Create user Succeeded";
            return true;
        }

        public bool Delete(string id, string currentUserId, out string errMsg)
        {
            var user = _uow.Set<ApplicationUser>().Find(id);
            if (user == null)
            {
                errMsg = "Can't find this user.";
                return false;
            }
            user.IsDeleted = true;
            user.DeleteBy = currentUserId;
            user.DeleteDate = DateTime.Now;
            _uow.Commit();
            errMsg = "success";
            return true;
        }

        public bool Edit(UserViewModel model, out string errMsg)
        {
            var user = _uow.Set<ApplicationUser>().Find(model.Id);
            if (user == null)
            {
                errMsg = "Can't find this user.";
                return false;
            }
            user.Name = model.Name;
            user.Email = model.Email;
            user.EmailConfirmed = !string.IsNullOrEmpty(model.Email) || model.EmailConfirmed;
            user.IsSuperAdmin = model.IsSuperAdmin;
            user.PhoneNumber = model.PhoneNumber;
            user.PhoneNumberConfirmed = !string.IsNullOrEmpty(model.PhoneNumber) || model.EmailConfirmed;
            _uow.Commit();
            var roles = _userManager.GetRolesAsync(user).Result;
            if (roles.FirstOrDefault() != model.Role)
            {
                if (roles.Count > 0)
                {
                    var r = _userManager.RemoveFromRolesAsync(user, roles).Result;
                }
                if (!string.IsNullOrEmpty(model.Role))
                {
                    var result2 = _userManager.AddToRoleAsync(user, model.Role).Result;
                    if (!result2.Succeeded)
                    {
                        errMsg = string.Join(", ", result2.Errors);
                        return true;
                    }
                }
            }

            errMsg = "Update user Succeeded";
            return true;
        }

        public UserViewModel GetById(string id)
        {
            var user = _uow.Set<ApplicationUser>().Find(id);
            if (user == null)
            {
                return null;
            }
            var roles = _userManager.GetRolesAsync(user).Result;
            return new UserViewModel(user, roles == null ? "" : roles.FirstOrDefault());
        }

        public UserProfileViewModel GetProfile(string id)
        {
            var user = _uow.Set<ApplicationUser>().Find(id);
            if (user == null)
            {
                return null;
            }
            var roles = _userManager.GetRolesAsync(user).Result;
          
            var loginLogs = _uow.Set<LoginLog>().Where(x => x.UserName == user.UserName).OrderByDescending(x => x.CreateDate).Take(10).ToList().Select(x => new LoginLogViewModel(x));
            return new UserProfileViewModel
            {
                User = new UserViewModel(user, roles == null ? "" : roles.FirstOrDefault()),
                LoginLogs = loginLogs,
            };
        }

        public bool Lock(string id, out string errMsg)
        {
            var user = _uow.Set<ApplicationUser>().Find(id);
            if (user == null)
            {
                errMsg = "Can't find this user";
                return false;
            }
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTime.Now.AddYears(100);
            _uow.Commit();
            errMsg = "Success";
            return true;
        }

        public PagedList<UserViewModel> Query(string key, int pageIndex, int pageSize)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            var query = _uow.Set<ApplicationUser>().AsQueryable();
            if (!string.IsNullOrEmpty(key))
            {
                query = query.Where(x => x.Name.Contains(key) ||
                                         x.UserName.Contains(key) ||
                                         x.Email.Contains(key));
            }
            var count = query.Count();
            var limit = pageSize;
            var offset = (pageIndex - 1) * limit;
            var items = new List<UserViewModel>();
            var data = query.OrderByDescending(x => x.CreateDate).Skip(offset).Take(limit).ToList();
            foreach (var d in data)
            {
                var roles = _userManager.GetRolesAsync(d).Result;
                items.Add(new UserViewModel(d, roles == null ? "" : roles.FirstOrDefault()));
            }
            return new PagedList<UserViewModel>(items, pageIndex, pageSize, count);
        }

        public bool ResetPassword(string id, string newPassword, out string errMsg)
        {
            if (string.IsNullOrEmpty(newPassword))
            {
                errMsg = "Please enter new password.";
                return false;
            }
            var user = _userManager.FindByIdAsync(id).Result;
            if (user == null)
            {
                errMsg = "Can't find user";
                return false;
            }
            var result1 = _userManager.RemovePasswordAsync(user).Result;
            if (result1.Succeeded == false)
            {
                errMsg = "Can't reset password, please try again later.";
                return false;
            }
            var result2 = _userManager.AddPasswordAsync(user, newPassword).Result;
            if (result2.Succeeded == false)
            {
                errMsg = "Can't reset password,  because occurred error:" + result2.Errors.FirstOrDefault().Description;
                return false;
            }
            errMsg = "Success";
            return true;
        }


        public bool UnLock(string id, out string errMsg)
        {
            var user = _uow.Set<ApplicationUser>().Find(id);
            if (user == null)
            {
                errMsg = "Can't find this user";
                return false;
            }
            user.LockoutEnabled = false;
            user.LockoutEnd = null;
            _uow.Commit();
            errMsg = "Success";
            return true;
        }

        public bool UploadAvatar(string id, string subFolderName, IFormFile file, out string errMsg)
        {
            if (file == null)
            {
                errMsg = "Please upload image";
                return false;
            }
            if (file.Length <= 0)
            {
                errMsg = "Please upload a valid image";
                return false;
            }
            var user = _uow.Set<ApplicationUser>().Find(id);
            if (user == null)
            {
                errMsg = "Can find user";
                return false;
            }
            FileInfo info = new FileInfo(file.FileName);
            var fileExt = info.Extension;
            var fileName = string.Format("{0}{1}", Guid.NewGuid(), fileExt);
            using (var sm = file.OpenReadStream())
            {
                var bytes = new byte[file.Length];
                sm.Read(bytes, 0, (int)file.Length);
                var ret = _imageService.Create(fileName, bytes, subFolderName);
                if (ret == null)
                {
                    errMsg = "Can't upload " + file.FileName;
                    return false;
                }
                else
                {
                    user.Avatar = _uow.Set<Image>().Find(ret.Id);
                    _uow.Commit();
                    errMsg = "Success";
                    return true;
                }
            }
        }

    }
}
