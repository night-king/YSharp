using System;

namespace YSharp.Domain
{
    public class BaseEntity<T>
    {
        public T Id { set; get; }

        public DateTime CreateDate { set; get; }

        public BaseEntity()
        {
            CreateDate = DateTime.Now;
        }
    }
}
