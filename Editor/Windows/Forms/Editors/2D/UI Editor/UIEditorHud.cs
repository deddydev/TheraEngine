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

            PrimitiveData data1 = PrimitiveData.FromTriangles(VertexShaderDesc.PosTex(), lines);
            PrimitiveData data2 = PrimitiveData.FromTriangles(VertexShaderDesc.PosTex(), lines);

            GLSLScript script1 = Engine.Files.LoadEngineShader("Outline2DUnlitForward.fs", EGLSLType.Fragment);
            GLSLScript script2 = Engine.Files.LoadEngineShader("Outline2DUnlitForward.fs", EGLSLType.Fragment);

            TMaterial highlightMat = new TMaterial("OutlineMaterial", new ShaderVar[] 
            {
                new ShaderVec2(Vec2.Zero, "Size"),
                new ShaderFloat(5.0f, "LineWidth"),

            }, script1);

            _highlightMesh.Mesh = new PrimitiveManager(data1, highlightMat);

            TMaterial boundsMat = new TMaterial("OutlineMaterial", new ShaderVar[] 
            {
                new ShaderVec2(Vec2.Zero, "Size"),
                new ShaderFloat(5.0f, "LineWidth"),
            }, script2);

            _uiBoundsMesh.Mesh = new PrimitiveManager(data2, boundsMat);

            BaseTransformComponent.WorldTransformChanged += BaseTransformComponent_WorldTransformChanged;
        }

        private void BaseTransformComponent_WorldTransformChanged(TheraEngine.Components.SceneComponent obj)
        {
            if (_targetHud?.RootComponent is UICanvasComponent canvas)
            {
                canvas.Size = _previewResolution;
                _uiBoundsMesh.Mesh.Material.Parameter<ShaderVec2>(0).Value = canvas.Size * BaseTransformComponent.Scale;
            }
            if (_highlightedComp is UIBoundableComponent bound)
                _highlightMesh.Mesh.Material.Parameter<ShaderVec2>(0).Value = bound.Size * BaseTransformComponent.Scale;
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
                    _uiBoundsMesh.Mesh.Material.Parameter<ShaderVec2>(0).Value = canvas.Size * BaseTransformComponent.Scale;
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
                        _uiBoundsMesh.Mesh.Material.Parameter<ShaderVec2>(0).Value = canvas.Size * BaseTransformComponent.Scale;
                    }
                }
                else
                {

                }
            }
        }

        protected override bool IsDragging => _dragComp != null;

        private UIComponent _dragComp, _selectedComp, _highlightedComp;
        private IUserInterface _targetHud;
        
        public override void RegisterInput(InputInterface input)
        {
            base.RegisterInput(input);
        }
        protected override void OnLeftClickDown()
        {
            if (_selectedComp == _highlightedComp)
            {
                _dragComp = _selectedComp;
            }
            else
            {
                _selectedComp = _highlightedComp;
                _dragComp = null;
            }
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
            if (IsDragging)
                return;

            UIComponent target = (_targetHud?.RootComponent as UICanvasComponent)?.FindDeepestComponent(CursorPositionWorld(), false);
            if (target != _highlightedComp)
            {
                Engine.PrintLine(target?.Name ?? "Nothing");
                _highlightedComp = target;
                if (_highlightedComp is UIBoundableComponent bound)
                    _highlightMesh.Mesh.Material.Parameter<ShaderVec2>(0).Value = bound.Size * BaseTransformComponent.Scale;
            }
            //UIComponentHighlighted?.Invoke(_highlightedComp);
        }

        protected override void HandleDragItem()
        {
            Vec2 diff = GetWorldCursorDiff(CursorPosition());
            _dragComp.LocalTranslation += diff / BaseTransformComponent.ScaleX;
        }
        protected override void AddRenderables(RenderPasses passes)
        {
            if (_selectedComp != null)
            {
                Matrix4 mtx = _dragComp.WorldMatrix;
                if (_dragComp is UIBoundableComponent bound)
                    mtx = mtx * Matrix4.CreateScale(bound.Width, bound.Height, 0.0f);

                _highlightMesh.WorldMatrix = mtx;
                passes.Add(_highlightMesh);
            }
            else if (_highlightedComp != null)
            {
                Matrix4 mtx = _highlightedComp.WorldMatrix;
                if (_highlightedComp is UIBoundableComponent bound)
                    mtx = mtx * Matrix4.CreateScale(bound.Width, bound.Height, 0.0f);

                _highlightMesh.WorldMatrix = mtx;
                passes.Add(_highlightMesh);
            }

            UICanvasComponent canvas = _targetHud?.RootComponent as UICanvasComponent;
            _uiBoundsMesh.WorldMatrix = canvas.WorldMatrix * Matrix4.CreateScale(canvas.Size);
            
            passes.Add(_uiBoundsMesh);
        }
    }
}
