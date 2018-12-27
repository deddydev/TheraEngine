﻿using System;
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
        /// <summary>
        /// Called before any viewports are rendered.
        /// </summary>
        protected virtual void GlobalPreRender() { OnPreRender(); }
        protected void OnPreRender() => PreRendered?.Invoke();
        /// <summary>
        /// Called after all viewports have been rendered.
        /// </summary>
        protected virtual void GlobalPostRender() { OnPostRender(); }
        protected void OnPostRender() => PostRendered?.Invoke();

        protected override void OnUpdate()
        {
            foreach (Viewport v in _viewports)
                v.Update(GetScene(v), GetCamera(v), GetFrustum(v));
        }
        public override void SwapBuffers()
        {
            foreach (Viewport v in _viewports)
            {
                GetScene(v)?.PreRenderSwap();
                v.SwapBuffers();
            }
        }
        protected override void OnRender()
        {
            Camera camera;
            BaseScene scene;

            GlobalPreRender();
            foreach (Viewport viewport in _viewports)
            {
                camera = GetCamera(viewport);
                scene = GetScene(viewport);

                viewport.HUD?.ScreenSpaceUIScene?.PreRender(viewport, viewport.HUD.ScreenOverlayCamera);
                scene?.PreRender(viewport, camera);
                viewport.Render(scene, camera, null);
            }
            GlobalPostRender();
        }
    }
}
