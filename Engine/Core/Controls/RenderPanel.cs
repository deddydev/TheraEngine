using System;
using System.Windows.Forms;
using TheraEngine.Core.Extensions;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine
{
    /// <summary>
    /// Used for rendering using any rasterizer that inherits from AbstractRenderer.
    /// Supports a 2D or 3D scene processor.
    /// </summary>
    public abstract class RenderPanel<T> : BaseRenderPanel where T : BaseScene
    {
        public event Action PreRendered, PostRendered;
        /// <summary>
        /// Returns the scene to render. A scene contains renderable objects and a management tree.
        /// </summary>
        /// <param name="v">The current viewport that is to be rendered.</param>
        /// <returns>The scene to render.</returns>
        protected abstract T GetScene(Viewport v);
        /// <summary>
        /// Returns the camera to render the scene from for this frame.
        /// By default, returns the viewport's camera.
        /// </summary>
        /// <param name="v">The current viewport that is to be rendered.</param>
        /// <returns>The camera to render the scene from for this frame.</returns>
        protected virtual Camera GetCamera(Viewport v) => v.Camera;
        /// <summary>
        /// Returns the view frustum to cull the scene with.
        /// By defualt, returns the current camera's frustum.
        /// </summary>
        /// <param name="v">The current viewport that is to be rendered.</param>
        /// <returns>The frustum to cull the scene with.</returns>
        protected virtual Frustum GetFrustum(Viewport v) => GetCamera(v)?.Frustum;
        protected virtual void PreRender() => PreRendered?.Invoke();
        protected virtual void PostRender() => PostRendered?.Invoke();
        protected override void OnUpdate()
        {
            foreach (Viewport v in _viewports)
                v.Update(GetScene(v), GetCamera(v), GetFrustum(v));
        }
        public override void SwapBuffers()
        {
            foreach (Viewport v in _viewports)
                v.SwapBuffers(GetScene(v));
        }
        protected override void OnRender()
        {
            if (!this.IsOnScreen())
                return;
            PreRender();
            foreach (Viewport v in _viewports)
                v.Render(GetScene(v), GetCamera(v), null);
            PostRender();
        }
    }
}
