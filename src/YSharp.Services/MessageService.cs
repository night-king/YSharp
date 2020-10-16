using YSharp.Domain;
using YSharp.Models;
using YSharp.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YSharp.Services
{
    public interface IMessageService
    {
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        bool Create(MessageViewModel model, out string errMsg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        MessageViewModel GetById(string id);

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="senderId"></param>
        /// <param name="accepterId"></param>
        /// <returns></returns>
        PagedList<MessageViewModel> Query(string key, int pageIndex, int pageSize, string senderId = "", string accepterId = "");
    }
    public class MessageService : IMessageService
    {
        readonly IUnitOfWork _uow;
        public MessageService(IUnitOfWork uow)
        {
            this._uow = uow;
        }
        public bool Create(MessageViewModel model, out string errMsg)
        {
            var sender = string.IsNullOrEmpty(model.SenderId) ? null : _uow.Set<ApplicationUser>().Find(model.SenderId);
            var accepter = string.IsNullOrEmpty(model.AccepterId) ? null : _uow.Set<ApplicationUser>().Find(model.AccepterId);
            if (sender == null)
            {
                errMsg = "Can't find sender";
                return false;
            }
            if (accepter == null)
            {
                errMsg = "Can't find accepter";
                return false;
            }
            var entity = new Message
            {
                Id = RandomId.CreateEnhance(),
                Content = model.Content,
                Sender = sender,
                Accepter = accepter,
                CreateDate = DateTime.Now,
            };
            _uow.Set<Message>().Add(entity);
            _uow.Commit();
            errMsg = "Success";
            return true;
        }

        public MessageViewModel GetById(string id)
        {
            var entity = _uow.Set<Message>().Find(id);
            if (entity == null)
            {
                return null;
            }
            return new MessageViewModel(entity);
        }

        public PagedList<MessageViewModel> Query(string key, int pageIndex, int pageSize, string senderId = "", string accepterId = "")
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            var query = _uow.Set<Message>().AsQueryable();
            if (!string.IsNullOrEmpty(key))
            {
                query = query.Where(x => x.Content.Contains(key));
            }
            if (!string.IsNullOrEmpty(senderId))
            {
                query = query.Where(x => x.Sender.Id == senderId);
            }
            if (!string.IsNullOrEmpty(accepterId))
            {
                query = query.Where(x => x.Accepter.Id == accepterId);
            }
            var count = query.Count();
            var limit = pageSize;
            var offset = (pageIndex - 1) * limit;
            var items = query.OrderByDescending(x => x.CreateDate).Skip(offset).Take(limit).ToList().Select(x => new MessageViewModel(x));
            return new PagedList<MessageViewModel>(items, pageIndex, pageSize, count);

        }
    }
}
