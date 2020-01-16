using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;
using Extensions;
using TheraEngine.Input.Devices;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Actors;
using TheraEngine.Input;
using System.Collections;
using TheraEngine.Core;
using System.Linq;

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
        private IUIInteractableComponent _focusedComponent;

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

        public Vec2 CursorPositionWorld { get; set; }

        public bool PreRenderEnabled { get; }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            base.OnRecalcLocalTransform(out localTransform, out inverseLocalTransform);
        }

        protected override void OnResizeLayout(BoundingRectangleF parentRegion)
        {
            //Engine.PrintLine($"UI CANVAS: {parentBounds.X.Rounded(2)}x{parentBounds.Y.Rounded(2)}");

            ScreenSpaceUIScene.Resize(parentRegion.Extents);
            ScreenSpaceCamera.Resize(parentRegion.Width, parentRegion.Height);

            base.OnResizeLayout(parentRegion);
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
            => ScreenSpaceUIScene?.PreRenderUpdate(ScreenSpaceRenderPasses, null, ScreenSpaceCamera);
        public void SwapInScreenSpace()
        {
            ScreenSpaceUIScene?.GlobalSwap();
            ScreenSpaceRenderPasses.SwapBuffers();
        }

        internal List<I2DRenderable> FindAllIntersecting(Vec2 viewportPoint) => throw new NotImplementedException();

        protected internal override void RegisterInputs(InputInterface input)
        {
            input.RegisterMouseMove(MouseMove, EMouseMoveType.Absolute, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick, EButtonInputType.Pressed, OnClick, EInputPauseType.TickAlways);

            //input.RegisterAxisUpdate(GamePadAxis.LeftThumbstickX, OnLeftStickX, false, EInputPauseType.TickOnlyWhenPaused);
            //input.RegisterAxisUpdate(GamePadAxis.LeftThumbstickY, OnLeftStickY, false, EInputPauseType.TickOnlyWhenPaused);
            //input.RegisterButtonEvent(GamePadButton.DPadUp, ButtonInputType.Pressed, OnDPadUp, EInputPauseType.TickOnlyWhenPaused);
            //input.RegisterButtonEvent(GamePadButton.FaceDown, ButtonInputType.Pressed, OnGamepadSelect, InputPauseType.TickOnlyWhenPaused);
            //input.RegisterButtonEvent(GamePadButton.FaceRight, ButtonInputType.Pressed, OnBackInput, EInputPauseType.TickOnlyWhenPaused);

            base.RegisterInputs(input);
        }

        protected virtual void OnLeftStickX(float value) { }
        protected virtual void OnLeftStickY(float value) { }

        /// <summary>
        /// Called on either left click or A button.
        /// Default behavior will OnClick the currently focused/highlighted UI component, if anything.
        /// </summary>
        //protected virtual void OnSelectInput()
        //{
        //_focusedComponent?.OnSelect();
        //}
        protected virtual void OnScrolledInput(bool up)
        {
            //_focusedComponent?.OnScrolled(up);
        }
        protected virtual void OnBackInput()
        {
            //_focusedComponent?.OnBack();
        }
        protected virtual void OnDPadUp()
        {

        }

        private class Comparer : IComparer<I2DRenderable>, IComparer
        {
            public int Compare(I2DRenderable x, I2DRenderable y)
            {
                IRenderInfo2D left = x.RenderInfo;
                IRenderInfo2D right = y.RenderInfo;

                if (left.LayerIndex > right.LayerIndex)
                    return -1;

                if (right.LayerIndex > left.LayerIndex)
                    return 1;

                if (left.IndexWithinLayer > right.IndexWithinLayer)
                    return -1;

                if (right.IndexWithinLayer > left.IndexWithinLayer)
                    return 1;

                return 0;
            }
            public int Compare(object x, object y)
                => Compare((I2DRenderable)x, (I2DRenderable)y);
        }
        public IUIInteractableComponent FocusedComponent
        {
            get => _focusedComponent;
            set
            {
                if (_focusedComponent != null)
                    _focusedComponent.IsFocused = false;
                _focusedComponent = value;
                if (_focusedComponent != null)
                    _focusedComponent.IsFocused = true;
            }
        }

        IUIInteractableComponent DeepestInteractable { get; set; }

        private SortedSet<I2DRenderable> LastInteractableIntersections = new SortedSet<I2DRenderable>(new Comparer());
        private SortedSet<I2DRenderable> InteractableIntersections = new SortedSet<I2DRenderable>(new Comparer());

        protected bool InteractablePredicate(I2DRenderable item) => item is IUIInteractableComponent;
        protected virtual void MouseMove(float x, float y)
        {
            Vec2 newPos = GetCursorPositionWorld();
            if (CursorPositionWorld.DistanceToSquared(newPos) < 0.001f) 
                return;

            CursorPositionWorld = newPos;

            var tree = ScreenSpaceUIScene?.RenderTree;
            if (tree != null)
            {
                tree.FindAllIntersectingSorted(CursorPositionWorld, InteractableIntersections, InteractablePredicate);
                DeepestInteractable = InteractableIntersections.Min as IUIInteractableComponent;

                InteractableIntersections.Union(LastInteractableIntersections).ForEach(ValidateIntersection);
                THelpers.Swap(ref LastInteractableIntersections, ref InteractableIntersections);

                if (DeepestInteractable != null)
                    Engine.Out(DeepestInteractable.ToString());
            }
        }

        private void ValidateIntersection(I2DRenderable obj)
        {
            if (LastInteractableIntersections.Contains(obj))
            {
                if (!InteractableIntersections.Contains(obj))
                {
                    //Lost mouse over
                    IUIInteractableComponent inter = obj as IUIInteractableComponent;

                    inter.IsMouseOver = false;
                    inter.IsMouseDirectlyOver = false;
                }
            }
            else
            {
                if (InteractableIntersections.Contains(obj))
                {
                    //Got mouse over
                    IUIInteractableComponent inter = obj as IUIInteractableComponent;

                    inter.IsMouseOver = true;
                    inter.IsMouseDirectlyOver = obj == DeepestInteractable;
                }
            }
        }

        private void OnClick()
        {
            FocusedComponent = DeepestInteractable;
        }
        private Vec2 GetCursorPositionWorld()
        {
            LocalPlayerController controller = null;
            if (OwningActor is IPawn pawn)
            {
                if (pawn is IUserInterfacePawn ui && ui.OwningPawn is IPawn uiOwner)
                    controller = uiOwner.LocalPlayerController ?? pawn.LocalPlayerController;
                else
                    controller = pawn.LocalPlayerController;
            }

            Viewport v = controller?.Viewport;
            return v?.ScreenToWorld(v.CursorPosition()).Xy ?? Vec2.Zero;
        }
    }
}
