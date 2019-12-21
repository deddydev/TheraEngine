using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Rendering.UI
{
    public interface IUICanvasComponent : IUIBoundableComponent
    {
        ECanvasDrawSpace DrawSpace { get; set; }
        float CameraDrawSpaceDistance { get; set; }
        IScene2D ScreenSpaceUIScene { get; }
        OrthographicCamera ScreenSpaceCamera { get; }
        RenderPasses ScreenSpaceRenderPasses { get; set; }

        void RenderInScreenSpace(Viewport viewport, QuadFrameBuffer fbo);
    }
    public enum ECanvasDrawSpace
    {
        /// <summary>
        /// Canvas is drawn on top of the viewport.
        /// </summary>
        Screen,
        /// <summary>
        /// Canvas is drawn in front of the camera.
        /// </summary>
        Camera,
        /// <summary>
        /// Canvas is drawn in the world like any other actor.
        /// Camera is irrelevant.
        /// </summary>
        World,
    }
    public class UICanvasComponent : UIBoundableComponent, IUICanvasComponent, IPreRendered
    {
        public UICanvasComponent()
        {
            ScreenSpaceCamera = new OrthographicCamera(Vec3.One, Vec3.Zero, Rotator.GetZero(), Vec2.Zero, -0.5f, 0.5f);
            ScreenSpaceCamera.SetOriginBottomLeft();
            ScreenSpaceCamera.Resize(1, 1);

            _screenSpaceUIScene = new Scene2D();
        }

        private float _cameraDrawSpaceDistance = 0.1f;
        private ECanvasDrawSpace _drawSpace = ECanvasDrawSpace.Screen;
        private IScene2D _screenSpaceUIScene;

        public ECanvasDrawSpace DrawSpace
        {
            get => _drawSpace;
            set => Set(ref _drawSpace, value);
        }
        public float CameraDrawSpaceDistance
        {
            get => _cameraDrawSpaceDistance;
            set => Set(ref _cameraDrawSpaceDistance, value);
        }

        [Browsable(false)]
        public OrthographicCamera ScreenSpaceCamera { get; }
        [Browsable(false)]
        public IScene2D ScreenSpaceUIScene => _screenSpaceUIScene;
        [Browsable(false)]
        public RenderPasses ScreenSpaceRenderPasses { get; set; } = new RenderPasses();

        public bool PreRenderEnabled { get; }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            base.OnRecalcLocalTransform(out localTransform, out inverseLocalTransform);
        }

        public override void ArrangeChildren(Vec2 translation, Vec2 parentBounds)
        {
            ScreenSpaceUIScene.Resize(parentBounds);
            ScreenSpaceCamera.Resize(parentBounds.X, parentBounds.Y);

            base.ArrangeChildren(translation, parentBounds);
        }

        public void PreRenderUpdate(ICamera camera)
        {

        }
        public void PreRenderSwap()
        {

        }
        public void PreRender(Viewport viewport, ICamera camera)
        {

        }

        public void RenderInScreenSpace(Viewport viewport, QuadFrameBuffer fbo) 
            => ScreenSpaceUIScene?.Render(ScreenSpaceRenderPasses, ScreenSpaceCamera, viewport, fbo);
        public void UpdateInScreenSpace()
            => ScreenSpaceUIScene?.Update(ScreenSpaceRenderPasses, null, ScreenSpaceCamera);
        public void SwapInScreenSpace()
        {
            ScreenSpaceUIScene?.GlobalSwap();
            ScreenSpaceRenderPasses.SwapBuffers();
        }

        internal List<I2DRenderable> FindAllIntersecting(Vec2 viewportPoint) => throw new NotImplementedException();
    }
}
