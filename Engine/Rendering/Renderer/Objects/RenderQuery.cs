using System;
using System.Threading.Tasks;

namespace TheraEngine.Rendering
{
    public class RenderQuery : BaseRenderObject
    {
        public EQueryTarget? CurrentQuery { get; private set; } = null;
        public RenderQuery() : base(EObjectType.Query) { }
        public void BeginQuery(EQueryTarget target)
        {
            if (CurrentQuery != null)
                EndQuery();
            CurrentQuery = target;
            Engine.Renderer.BeginQuery(BindingId, target);
        }
        public void EndQuery()
        {
            if (CurrentQuery == null)
                return;
            Engine.Renderer.EndQuery(CurrentQuery.Value);
            CurrentQuery = null;
        }
        public int EndAndGetQueryInt()
        {
            EndQuery();
            return GetQueryObjectInt(EGetQueryObject.QueryResult);
        }
        public long EndAndGetQueryLong()
        {
            EndQuery();
            return GetQueryObjectLong(EGetQueryObject.QueryResult);
        }
        public void QueryCounter()
        {
            if (CurrentQuery != EQueryTarget.Timestamp)
                return;
            Engine.Renderer.QueryCounter(BindingId);
        }
        public long GetQueryObjectLong(EGetQueryObject obj)
            => Engine.Renderer.GetQueryObjectLong(BindingId, obj);
        public int GetQueryObjectInt(EGetQueryObject obj)
            => Engine.Renderer.GetQueryObjectInt(BindingId, obj);
        public void AwaitResult()
        {
            int result = 0;
            while (result == 0)
                result = GetQueryObjectInt(EGetQueryObject.QueryResultAvailable);
        }
        public void AwaitResult(Action<RenderQuery> onReady)
        {
            if (onReady == null)
                AwaitResult();
            else
                Task.Run(() => AwaitResult()).ContinueWith(t => onReady(this));
        }
        public async Task AwaitResultAsync()
        {
            await Task.Run(() => AwaitResult());
        }
        public async Task<int> AwaitIntResultAsync()
        {
            await Task.Run(() => AwaitResult());
            return GetQueryObjectInt(EGetQueryObject.QueryResult);
        }
        public async Task<long> AwaitLongResultAsync()
        {
            await Task.Run(() => AwaitResult());
            return GetQueryObjectLong(EGetQueryObject.QueryResult);
        }
    }
}
