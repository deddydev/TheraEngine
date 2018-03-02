using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine
{
    /// <summary>
    /// Used for rendering using any rasterizer that inherits from AbstractRenderer.
    /// Supports a 2D or 3D scene processor.
    /// </summary>
    public abstract class RenderPanel<T> : BaseRenderPanel where T : Scene
    {
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
        protected virtual void PreRender() { }
        protected virtual void PostRender() { }
        protected override void OnRender()
        {
            PreRender();
            _context.BeginDraw();
            foreach (Viewport v in _viewports)
                v.Render(GetScene(v), GetCamera(v), GetFrustum(v), null);
            //_globalHud?.Render();
            _context.EndDraw();
            PostRender();
        }
    }
}
