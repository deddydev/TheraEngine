using System;
using TheraEngine.Rendering;

namespace TheraEngine
{
    public class BasicRenderPanel : BaseRenderPanel
    {
        public override int MaxViewports => 1;

        public Scene3D Scene { get; } = new Scene3D();
        
        //protected override void OnLoad(EventArgs e)
        //{
        //    base.OnLoad(e);
        //    if (!DesignMode)
        //        AddViewport();
        //}

        protected virtual void PreRender() { }
        protected virtual void PostRender() { }
        protected override void OnRender()
        {
            //Viewport v = _viewports[0];

            //PreRender();
            //_context.BeginDraw();
            //v.Render(Scene, v.Camera, v.Camera.Frustum);
            //_context.EndDraw();
            //PostRender();
        }
    }
}
