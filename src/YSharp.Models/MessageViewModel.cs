using YSharp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace YSharp.Models
{
    public class MessageViewModel
    {
        public MessageViewModel() { }
        public MessageViewModel(Message entity)
        {
            this.Id = entity.Id;
            this.Content = entity.Content;
            this.SenderId = entity.Sender == null ? "" : entity.Sender.Id;
            this.SenderName = entity.Sender == null ? "" : entity.Sender.Name;
            this.SenderUserName = entity.Sender == null ? "" : entity.Sender.UserName;
            this.AccepterId = entity.Accepter == null ? "" : entity.Accepter.Id;
            this.AccepterName = entity.Accepter == null ? "" : entity.Accepter.Name;
            this.AccepterUserName = entity.Accepter == null ? "" : entity.Accepter.UserName;
            this.CreateDate = entity.CreateDate;
        }

        public string Id { set; get; }

        public string Content { set; get; }

        public string SenderId { set; get; }
        public string AccepterId { set; get; }

        public string SenderName { set; get; }
        public string SenderUserName { set; get; }

        public string AccepterName { set; get; }
        public string AccepterUserName { set; get; }

        public DateTime CreateDate { set; get; }

    }
}
