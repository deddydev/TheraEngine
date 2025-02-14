﻿using System;
using TheraEngine.Core;
using TheraEngine.Rendering;

namespace TheraEngine
{
    /// <summary>
    /// Used for rendering using any rasterizer that inherits from AbstractRenderer.
    /// Supports a 2D or 3D scene processor.
    /// </summary>
    public abstract class RenderHandler<T> : BaseRenderHandler where T : IScene
    {
        public event Action PreRendered, PostRendered;
        /// <summary>
        /// Returns the scene to render. A scene contains renderable objects and a management tree.
        /// </summary>
        /// <param name="v">The current viewport that is to be rendered.</param>
        /// <returns>The scene to render.</returns>
        //protected abstract T GetScene(Viewport v);
        /// <summary>
        /// Returns the camera to render the scene from for this frame.
        /// By default, returns the viewport's camera.
        /// </summary>
        /// <param name="v">The current viewport that is to be rendered.</param>
        /// <returns>The camera to render the scene from for this frame.</returns>
        //protected virtual ICamera GetCamera(Viewport v) => v.AttachedCamera;
        /// <summary>
        /// Returns the view frustum to cull the scene with.
        /// By defualt, returns the current camera's frustum.
        /// </summary>
        /// <param name="v">The current viewport that is to be rendered.</param>
        /// <returns>The frustum to cull the scene with.</returns>
        //protected virtual IVolume GetCullingVolume(Viewport v) => GetCamera(v)?.Frustum;
        /// <summary>
        /// Called before any viewports are rendered.
        /// </summary>
        protected virtual void GlobalPreRender() => OnPreRender();
        protected void OnPreRender() => PreRendered?.Invoke();
        /// <summary>
        /// Called after all viewports have been rendered.
        /// </summary>
        protected virtual void GlobalPostRender() => OnPostRender();
        protected void OnPostRender() => PostRendered?.Invoke();

        public override void PreRenderUpdate()
        {
            foreach (Viewport v in Viewports.Values)
                v.PreRenderUpdate();
        }
        public override void SwapBuffers()
        {
            foreach (Viewport v in Viewports.Values)
                v.PreRenderSwap();
        }
        public override void Render()
        {
            GlobalPreRender();
            foreach (Viewport viewport in Viewports.Values)
                viewport.Render();
            GlobalPostRender();
        }
    }
}
