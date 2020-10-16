using System;

namespace YSharp.Domain
{
    public class FakeDeleteEntity<T> : BaseEntity<T>
    {
        public bool IsDeleted { set; get; }

        public DateTime? DeleteDate { set; get; }

        public string DeleteBy { set; get; }

        public FakeDeleteEntity()
        {
            IsDeleted = false;
        }

    }
}
