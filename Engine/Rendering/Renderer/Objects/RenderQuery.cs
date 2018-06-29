using System;
using System.Threading.Tasks;

namespace TheraEngine.Rendering
{
    public class RenderQuery : BaseRenderObject
    {
        public RenderQuery() : base(EObjectType.Query) { }
        public void BeginQuery(EQueryTarget target)
            => Engine.Renderer.BeginQuery(BindingId, target);
        public void EndQuery(EQueryTarget target)
            => Engine.Renderer.EndQuery(target);
        public void QueryCounter()
            => Engine.Renderer.QueryCounter(BindingId);
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
    }
}
