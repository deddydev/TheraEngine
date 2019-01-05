using System;
using System.ComponentModel;
using System.Drawing;
using TheraEngine;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.UI;

namespace TheraEditor.Windows.Forms
{
    public delegate void DelUIComponentSelect(UIComponent comp);
    public class UIHudEditor : EditorUserInterface, I2DRenderable
    {
        public UIHudEditor() : base()
        {
            VertexQuad quad = VertexQuad.PosZQuad(1, true, -0.5f, false);
            VertexTriangle[] lines = quad.ToTriangles();
            PrimitiveData data = PrimitiveData.FromTriangles(VertexShaderDesc.JustPositions(), lines);
            TMaterial mat = TMaterial.CreateUnlitColorMaterialForward(Color.Yellow);
            //mat.RenderParams.DepthTest.Enabled = ERenderParamUsage.Disabled;
            _highlightMesh.Mesh = new PrimitiveManager(data, mat);
            _uiBoundsMesh.Mesh = new PrimitiveManager(data, mat);
        }
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
                else if (_targetHud != null)
                {
                    _baseTransformComponent.ChildComponents.Remove(_targetHud.RootComponent);
                }
                _targetHud = value;
                if (_targetHud != null)
                {
                    _baseTransformComponent.ChildComponents.Add(_targetHud.RootComponent);
                    IVec2 vec = LocalPlayerController.Viewport.Region.Extents;
                    UIDockableComponent comp = _targetHud.RootComponent as UIDockableComponent;
                    //comp.Bounds = vec;
                }
                else
                {

                }
            }
        }

        RenderInfo2D I2DRenderable.RenderInfo { get; } = new RenderInfo2D(ERenderPass.OnTopForward, 99, 0);
        [Browsable(false)]
        public BoundingRectangleF AxisAlignedRegion { get; } = new BoundingRectangleF();
        [Browsable(false)]
        public IQuadtreeNode QuadtreeNode { get; set; }
        private UIComponent _dragComp, _highlightedComp;
        private IUserInterface _targetHud;
        private Vec2 _minScale = new Vec2(0.1f), _maxScale = new Vec2(4.0f);
        private Vec2 _lastWorldPos = Vec2.Zero;
        //private Vec2 _lastFocusPoint = Vec2.Zero;
        private bool _rightClickDown = false;
        
        public override void OnSpawnedPostComponentSpawn()
        {
            base.OnSpawnedPostComponentSpawn();
            ScreenSpaceUIScene.Add(this);
        }
        public override void OnDespawned()
        {
            base.OnDespawned();
            ScreenSpaceUIScene.Remove(this);
        }
        protected override TMaterial GetBackgroundMaterial()
        {
            GLSLScript frag = Engine.Files.LoadEngineShader("MaterialEditorGraphBG.fs", EGLSLType.Fragment);
            return new TMaterial("MatEditorGraphBG", new ShaderVar[]
            {
                new ShaderVec3(new Vec3(0.15f, 0.18f, 0.23f), "LineColor"),
                new ShaderVec3(new Vec3(0.20f, 0.25f, 0.3f), "BGColor"),
                new ShaderFloat(1.0f, "Scale"),
                new ShaderFloat(0.1f, "LineWidth"),
                new ShaderVec2(new Vec2(0.0f), "Translation"),
            },
            frag);
        }
        
        public override void RegisterInput(InputInterface input)
        {
            input.RegisterKeyPressed(EKey.AltLeft, b => _altDown = b, EInputPauseType.TickAlways);
            input.RegisterKeyPressed(EKey.ControlLeft, b => _ctrlDown = b, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick, EButtonInputType.Pressed, LeftClickDown, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick, EButtonInputType.Released, LeftClickUp, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.RightClick, EButtonInputType.Pressed, RightClickDown, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.RightClick, EButtonInputType.Released, RightClickUp, EInputPauseType.TickAlways);
            input.RegisterMouseScroll(OnScrolledInput, EInputPauseType.TickAlways);
            input.RegisterMouseMove(MouseMove, EMouseMoveType.Absolute, EInputPauseType.TickAlways);
        }
        private bool _altDown = false;
        private bool _ctrlDown = false;
        internal void LeftClickDown()
        {
            _dragComp = _highlightedComp;
        }
        internal void LeftClickUp()
        {
            _dragComp = null;
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
            else if (_dragComp != null)
                HandleDragHudComp(pos, _dragComp);
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
            _baseTransformComponent.LocalTranslation += GetWorldCursorDiff(cursorPosScreen);

            TMaterial mat = _backgroundComponent.InterfaceMaterial;
            mat.Parameter<ShaderVec2>(4).Value = _baseTransformComponent.LocalTranslation;
        }
        private void HandleDragHudComp(Vec2 cursorPosScreen, UIComponent draggedComp)
        {
            Vec2 diff = GetWorldCursorDiff(cursorPosScreen);
            draggedComp.LocalTranslation += Vec3.TransformVector(diff, draggedComp.InverseWorldMatrix).Xy;
        }
        #endregion

        private void HighlightHud()
        {
            _highlightedComp = FindComponent();
        }
        protected override void OnScrolledInput(bool down)
        {
            Vec3 worldPoint = CursorPositionWorld();
            _baseTransformComponent.Zoom(down ? 0.1f : -0.1f, worldPoint.Xy, _minScale, _maxScale);

            TMaterial mat = _backgroundComponent.InterfaceMaterial;
            mat.Parameter<ShaderFloat>(2).Value = _baseTransformComponent.ScaleX;
            mat.Parameter<ShaderVec2>(4).Value = _baseTransformComponent.LocalTranslation;
        }

        private RenderCommandMesh2D _highlightMesh = new RenderCommandMesh2D();
        private RenderCommandMesh2D _uiBoundsMesh = new RenderCommandMesh2D();
        public void AddRenderables(RenderPasses passes)
        {
            if (_highlightedComp != null)
            {
                _highlightMesh.WorldMatrix = _highlightedComp.WorldMatrix;
                passes.Add(_highlightMesh, ERenderPass.TransparentForward);
            }

            _uiBoundsMesh.WorldMatrix = Matrix4.CreateScale(TargetHUD.Bounds);
            passes.Add(_uiBoundsMesh, ERenderPass.TransparentForward);
        }
    }
}
