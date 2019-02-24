using System;
using System.Drawing;
using System.IO;
using TheraEngine;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.UI;

namespace TheraEditor.Windows.Forms
{
    public delegate void DelUIComponentSelect(UIComponent comp);
    public class UIEditorUI : EditorUI2DBase, I2DRenderable
    {
        public UIEditorUI() : base()
        {
            VertexQuad quad = VertexQuad.PosZQuad(1, true, -0.5f, false);
            VertexTriangle[] lines = quad.ToTriangles();
            PrimitiveData data = PrimitiveData.FromTriangles(VertexShaderDesc.PosTex(), lines);
            GLSLScript script = Engine.Files.LoadEngineShader("Outline2DUnlitForward.fs", EGLSLType.Fragment);
            TMaterial highlightMat = new TMaterial("OutlineMaterial", new ShaderVar[] { new ShaderVec2(Vec2.Zero, "Size") }, script);
            _highlightMesh.Mesh = new PrimitiveManager(data, highlightMat);
            TMaterial boundsMat = new TMaterial("OutlineMaterial", new ShaderVar[] { new ShaderVec2(Vec2.Zero, "Size") }, script);
            _uiBoundsMesh.Mesh = new PrimitiveManager(data, boundsMat);
            BaseTransformComponent.WorldTransformChanged += BaseTransformComponent_WorldTransformChanged;
        }

        private void BaseTransformComponent_WorldTransformChanged(TheraEngine.Components.SceneComponent obj)
        {
            if (_targetHud?.RootComponent is UICanvasComponent canvas)
            {
                canvas.Size = _previewResolution;
                _uiBoundsMesh.Mesh.Material.Parameter<ShaderVec2>(0).Value = canvas.Size * BaseTransformComponent.ScaleX;
            }
        }

        private Vec2 _previewResolution = new Vec2(1920, 1080);
        public Vec2 PreviewResolution
        {
            get => _previewResolution;
            set
            {
                _previewResolution = value;
                if (_targetHud?.RootComponent is UICanvasComponent canvas)
                {
                    canvas.Size = _previewResolution;
                    _uiBoundsMesh.Mesh.Material.Parameter<ShaderVec2>(0).Value = canvas.Size * BaseTransformComponent.ScaleX;
                }
            }
        }

        public event DelUIComponentSelect UIComponentSelected;
        //public event DelUIComponentSelect UIComponentHighlighted;
        
        public IUserInterface TargetHUD
        {
            get => _targetHud;
            set
            {
                if (_targetHud == value)
                    return;
                else if (_targetHud != null)
                {
                    BaseTransformComponent.ChildComponents.Remove(_targetHud.RootComponent);
                }
                _targetHud = value;
                if (_targetHud != null)
                {
                    BaseTransformComponent.ChildComponents.Add(_targetHud.RootComponent);
                    IVec2 vec = LocalPlayerController.Viewport.Region.Extents;
                    if (_targetHud?.RootComponent is UICanvasComponent canvas)
                    {
                        canvas.Size = PreviewResolution;
                        _uiBoundsMesh.Mesh.Material.Parameter<ShaderVec2>(0).Value = canvas.Size * BaseTransformComponent.ScaleX;
                    }
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
            UIComponentSelected?.Invoke(_dragComp);
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
            max = PreviewResolution;
            return true;
        }
        protected override void HighlightScene()
        {
            _highlightedComp = (_targetHud?.RootComponent as UICanvasComponent)?.FindDeepestComponent(CursorPositionWorld());
            if (_highlightedComp is UIBoundableComponent bound)
                _highlightMesh.Mesh.Material.Parameter<ShaderVec2>(0).Value = bound.Size;

            //UIComponentHighlighted?.Invoke(_highlightedComp);
        }

        protected override void HandleDragItem()
        {
            Vec2 diff = GetWorldCursorDiff(CursorPosition());
            _dragComp.LocalTranslation += diff / BaseTransformComponent.ScaleX;
        }
        protected override void AddRenderables(RenderPasses passes)
        {
            if (_highlightedComp != null)
            {
                Matrix4 mtx = _highlightedComp.WorldMatrix;
                if (_highlightedComp is UIBoundableComponent bound)
                    mtx = mtx * Matrix4.CreateScale(bound.Width, bound.Height, 1.0f);
                else
                    _highlightMesh.WorldMatrix = mtx;
                passes.Add(_highlightMesh);
            }

            UICanvasComponent canvas = _targetHud?.RootComponent as UICanvasComponent;
            _uiBoundsMesh.WorldMatrix = 
                Matrix4.CreateTranslation(BaseTransformComponent.LocalTranslation) *
                Matrix4.CreateScale((canvas?.Size ?? PreviewResolution) * BaseTransformComponent.ScaleX);
            
            passes.Add(_uiBoundsMesh);
        }
    }
}
