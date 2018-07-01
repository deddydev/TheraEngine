using System.Threading.Tasks;

namespace System
{
    public static class TaskExtension
    {
        public static async Task<TBase> Generalized<TBase, TDerived>(this Task<TDerived> task) where TDerived : TBase => await task;
        public static T GetResultSynchronously<T>(this Task<T> task)
        {
            task.Wait();
            return task.Result;
        }
    }
}
