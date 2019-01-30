using System;
using System.ComponentModel;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine
{
    public class BasicRenderPanel : RenderPanel<Scene3D>
    {
        public override int MaxViewports => 1;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Scene3D Scene { get; } = new Scene3D();
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Camera Camera
        {
            get => _viewports.Count == 0 ? null : _viewports [0].Camera;
            set
            {
                GetOrAddViewport(Actors.ELocalPlayerIndex.One).Camera = value;
                value.Resize(Width, Height);
            }
        }

        //protected override void OnLoad(EventArgs e)
        //{
        //    base.OnLoad(e);
        //    AddViewport();
        //}
        //protected override void OnHandleCreated(EventArgs e)
        //{
        //    base.OnHandleCreated(e);
        //    AddViewport();
        //}
        //protected override void SetVisibleCore(bool value)
        //{
        //    base.SetVisibleCore(value);
        //}
        //protected override void OnParentVisibleChanged(EventArgs e)
        //{
        //    base.OnParentVisibleChanged(e);
        //}
        //protected override void OnVisibleChanged(EventArgs e)
        //{
        //    base.OnVisibleChanged(e);
        //    if (!Engine.DesignMode && Visible && _viewports.Count == 0)
        //        AddViewport();
        //}
        
        protected override Scene3D GetScene(Viewport v) => Scene;
    }
}
