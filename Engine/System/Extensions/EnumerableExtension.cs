using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System
{
    public class ThreadSafeEnumerator<T> : IEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;
        ReaderWriterLockSlim _lock;
        public ThreadSafeEnumerator(IEnumerator<T> inner, ReaderWriterLockSlim rwlock)
        {
            _inner = inner;
            _lock = rwlock;
            _lock.EnterReadLock();
        }
        ~ThreadSafeEnumerator() => Dispose();
        public void Dispose()
        {
            _lock.ExitReadLock();
            _lock = null;
        }
        public bool MoveNext() => _inner.MoveNext();
        public void Reset() => _inner.Reset();
        public T Current => _inner.Current;
        object IEnumerator.Current => Current;
    }
    public class ThreadSafeEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _inner;
        private ReaderWriterLockSlim _lock;
        public ThreadSafeEnumerable()
        {
            _lock = new ReaderWriterLockSlim();
        }
        public ThreadSafeEnumerable(IEnumerable<T> inner, ReaderWriterLockSlim rwlock)
        {
            _inner = inner;
            _lock = rwlock;
        }
        public IEnumerator<T> GetEnumerator()
            => new ThreadSafeEnumerator<T>(_inner.GetEnumerator(), _lock);
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
    public static class EnumerableExtension
    {
        public static ThreadSafeEnumerable<T> AsThreadSafeEnumerable<T>(this IEnumerable<T> enumerable, ReaderWriterLockSlim rwlock)
        {
            return new ThreadSafeEnumerable<T>(enumerable, rwlock);
        }
        public static ThreadSafeList<T> AsThreadSafeList<T>(this IEnumerable<T> enumerable)
        {
            return new ThreadSafeList<T>(enumerable);
        }
    }
}
