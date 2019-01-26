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

        protected override bool IsDragging => _dragComp != null;

        private UIComponent _dragComp, _highlightedComp;
        private IUserInterface _targetHud;
        
        public override void RegisterInput(InputInterface input)
        {
            base.RegisterInput(input);
        }
        protected override void OnLeftClickDown()
        {
            _dragComp = _highlightedComp;
        }
        protected override void OnLeftClickUp()
        {
            _dragComp = null;
        }
        
        private RenderCommandMesh2D _highlightMesh = new RenderCommandMesh2D(ERenderPass.TransparentForward);
        private RenderCommandMesh2D _uiBoundsMesh = new RenderCommandMesh2D(ERenderPass.TransparentForward);

        protected override bool GetFocusAreaMinMax(out Vec2 min, out Vec2 max)
        {
            min = Vec2.Zero;
            max = Vec2.Zero;
            return false;
        }
        protected override void HighlightScene()
        {
            _highlightedComp = FindComponent();
        }

        protected override void HandleDragItem()
        {
            Vec2 diff = GetWorldCursorDiff(CursorPosition());
            _dragComp.LocalTranslation += diff;
        }
        protected override void AddRenderables(RenderPasses passes)
        {
            if (_highlightedComp != null)
            {
                _highlightMesh.WorldMatrix = _highlightedComp.WorldMatrix;
                passes.Add(_highlightMesh);
            }

            _uiBoundsMesh.WorldMatrix = Matrix4.CreateScale(TargetHUD.Bounds);
            passes.Add(_uiBoundsMesh);
        }
    }
}
