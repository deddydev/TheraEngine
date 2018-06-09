using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TheraEngine;

namespace System
{
    public class ThreadSafeEnumerator<T> : IEnumerator<T>, IDisposable
    {
        private readonly IEnumerator<T> _inner;
        ReaderWriterLockSlim _lock;
        public ThreadSafeEnumerator(IEnumerator<T> inner, ReaderWriterLockSlim rwlock)
        {
            _inner = inner;
            _lock = rwlock;
            _lock.EnterReadLock();
        }
        //~ThreadSafeEnumerator() => Dispose();
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
        public static IEnumerable<T> AsThreadSafeEnumerable<T>(this IEnumerable<T> enumerable, ReaderWriterLockSlim rwlock)
        {
            return new ThreadSafeEnumerable<T>(enumerable, rwlock);
        }
        public static ThreadSafeList<T> AsThreadSafeList<T>(this IEnumerable<T> enumerable)
        {
            return new ThreadSafeList<T>(enumerable);
        }
        public static IEnumerable<TResult> SelectEvery<TElement, TResult>(
            this IEnumerable<TElement> source,
            int count,
            Func<List<TElement>, TResult> formatter)
        {
            return source.Split(count).Select(arg => formatter(arg.ToList()));
        }
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int size)
        {
            var i = 0;
            return
                from element in source
                group element by i++ / size into splitGroups
                select splitGroups.AsEnumerable();
        }
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            //try
            //{
                foreach (T item in enumeration)
                    action(item);
            //}
            //catch(Exception ex)
            //{
                //Engine.PrintLine(ex.ToString());
            //}
        }
    }
}
