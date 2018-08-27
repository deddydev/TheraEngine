using System;
using System.Collections.Generic;
using System.Linq;
using TheraEngine;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Files;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models.Materials.Functions;
using TheraEngine.Rendering.UI;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEditor.Windows.Forms
{
    public delegate void DelUIComponentSelect(UIComponent comp);
    public class UIHudEditor : UserInterface<UIMaterialRectangleComponent>, I2DRenderable
    {
        public UIHudEditor() : base() { }
        public UIHudEditor(Vec2 bounds) : base(bounds) { }

        public event DelUIComponentSelect UIComponentSelected;
        public event DelUIComponentSelect UIComponentHighlighted;
        
        public IUserInterface TargetHUD
        {
            get => _targetHud;
            set
            {
                if (_targetHud == value)
                    return;
                _targetHud = value;
                if (_targetHud != null)
                {

                }
                else
                {

                }
            }
        }

        RenderInfo2D I2DRenderable.RenderInfo { get; } = new RenderInfo2D(ERenderPass.OnTopForward, 0, 0);
        public BoundingRectangleF AxisAlignedRegion { get; } = new BoundingRectangleF();
        public IQuadtreeNode QuadtreeNode { get; set; }

        private IUserInterface _targetHud;
        private Vec2 _minScale = new Vec2(0.1f), _maxScale = new Vec2(4.0f);
        private Vec2 _lastWorldPos = Vec2.Zero;
        //private Vec2 _lastFocusPoint = Vec2.Zero;
        internal UIComponent _rootTransform;
        private bool _rightClickDown = false;

        protected override UIMaterialRectangleComponent OnConstruct()
        {
            UIMaterialRectangleComponent root = new UIMaterialRectangleComponent(GetGraphMaterial())
            {
                DockStyle = UIDockStyle.Fill,
                SideAnchorFlags = AnchorFlags.Right | AnchorFlags.Left | AnchorFlags.Top | AnchorFlags.Bottom
            };
            _rootTransform = new UIComponent();
            root.ChildComponents.Add(_rootTransform);
            return root;
        }
        public override void OnSpawnedPostComponentSpawn()
        {
            base.OnSpawnedPostComponentSpawn();
            UIScene.Add(this);
        }
        public override void OnDespawned()
        {
            base.OnDespawned();
            UIScene.Remove(this);
        }
        private TMaterial GetGraphMaterial()
        {
            GLSLShaderFile frag = Engine.LoadEngineShader("MaterialEditorGraphBG.fs", EShaderMode.Fragment);
            return new TMaterial("MatEditorGraphBG", new ShaderVar[]
            {
                new ShaderVec3(new Vec3(0.1f, 0.12f, 0.13f), "LineColor"),
                new ShaderVec3(new Vec3(0.25f, 0.27f, 0.3f), "BGColor"),
                new ShaderFloat(1.0f, "Scale"),
                new ShaderFloat(0.05f, "LineWidth"),
                new ShaderVec2(new Vec2(0.0f), "Translation"),
            },
            frag);
        }
        
        #region Input
        public override void RegisterInput(InputInterface input)
        {
            input.RegisterButtonPressed(EKey.AltLeft, b => _altDown = b, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(EKey.ControlLeft, b => _ctrlDown = b, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick, ButtonInputType.Pressed, LeftClickDown, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick, ButtonInputType.Released, LeftClickUp, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.RightClick, ButtonInputType.Pressed, RightClickDown, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.RightClick, ButtonInputType.Released, RightClickUp, EInputPauseType.TickAlways);
            input.RegisterMouseScroll(OnScrolledInput, EInputPauseType.TickAlways);
            input.RegisterMouseMove(MouseMove, MouseMoveType.Absolute, EInputPauseType.TickAlways);
        }
        private bool _altDown = false;
        private bool _ctrlDown = false;
        internal void LeftClickDown()
        {

        }
        internal void LeftClickUp()
        {

        }
        private void RightClickDown()
        {
            _rightClickDown = true;
            //_lastWorldPos = Camera.LocalPoint.Xy;
            _lastWorldPos = CursorPositionWorld();
        }
        private void RightClickUp()
        {
            _rightClickDown = false;
        }
        protected override void MouseMove(float x, float y)
        {
            Vec2 pos = CursorPosition();
            if (_rightClickDown)
                HandleDragView(pos);
            else
                HighlightHud();
            _cursorPos = pos;
        }

        #region Dragging
        private Vec2 GetWorldCursorDiff(Vec2 cursorPosScreen)
        {
            Vec2 screenPoint = Viewport.WorldToScreen(_lastWorldPos).Xy;
            screenPoint += cursorPosScreen - _cursorPos;
            Vec2 newFocusPoint = Viewport.ScreenToWorld(screenPoint).Xy;
            Vec2 diff = newFocusPoint - _lastWorldPos;
            _lastWorldPos = newFocusPoint;
            return diff;
        }
        private void HandleDragView(Vec2 cursorPosScreen)
        {
            _rootTransform.LocalTranslation += GetWorldCursorDiff(cursorPosScreen);

            TMaterial mat = RootComponent.InterfaceMaterial;
            mat.Parameter<ShaderVec2>(4).Value = _rootTransform.LocalTranslation;
        }
        private void HandleDragHudComp(Vec2 cursorPosScreen, UIBoundableComponent draggedComp)
        {
            Vec2 diff = GetWorldCursorDiff(cursorPosScreen);
            draggedComp.LocalTranslation += Vec3.TransformVector(diff, draggedComp.InverseWorldMatrix).Xy;
        }
        #endregion

        private void HighlightHud()
        {
            UIComponent comp = FindComponent();

            if (comp is UITextComponent)
                comp = (UIComponent)comp.ParentSocket;
            
        }
        protected override void OnScrolledInput(bool down)
        {
            Vec3 worldPoint = CursorPositionWorld();
            _rootTransform.Zoom(down ? 0.1f : -0.1f, worldPoint.Xy, _minScale, _maxScale);

            TMaterial mat = RootComponent.InterfaceMaterial;
            mat.Parameter<ShaderFloat>(2).Value = _rootTransform.ScaleX;
            mat.Parameter<ShaderVec2>(4).Value = _rootTransform.LocalTranslation;
        }

        public void AddRenderables(RenderPasses passes)
        {

        }
        #endregion
    }
}
