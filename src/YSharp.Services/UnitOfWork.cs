using YSharp.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace YSharp.Services
{
    public interface IUnitOfWork : IDisposable
    {
        DbSet<T> Set<T>() where T : class;

        void Commit(int retryCount = 0);

    }

    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public DbSet<T> Set<T>() where T : class
        {
            return _context.Set<T>();
        }

        public void Commit(int retryCount = 0)
        {
            var count = 0;
            while (count < retryCount + 1)
            {
                try
                {
                    _context.SaveChanges();
                    break;
                }
                catch (DbUpdateConcurrencyException)
                {
                    count++;
                    Thread.Sleep(100);
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    if (_context != null)
                    {
                        _context.Dispose();
                        _context = null;
                    }
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~UnitOfWork() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    public static class UnitOfWorkExtensions
    {
        public static void Remove<T>(this IUnitOfWork uow, IEnumerable<T> items) where T : class
        {
            foreach (var item in items)
            {
                uow.Set<T>().Remove(item);
            }
        }

        public static void Remove<T>(this IUnitOfWork uow, Expression<Func<T, bool>> selector) where T : class
        {
            foreach (var item in uow.Set<T>().Where(selector).ToList())
            {
                uow.Set<T>().Remove(item);
            }
        }

        public static void AddRange<T>(this DbSet<T> dbSet, params T[] items) where T : class
        {
            foreach (var item in items)
            {
                dbSet.Add(item);
            }
        }
    }
}