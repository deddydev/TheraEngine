using System;
using System.Drawing;
using System.IO;
using TheraEngine;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Core.Maths.Transforms;
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
            PrimitiveData data3 = PrimitiveData.FromTriangles(VertexShaderDesc.PosTex(), lines);
            
            GLSLScript script1 = Engine.Files.LoadEngineShader("Outline2DUnlitForward.fs", EGLSLType.Fragment);
            GLSLScript script2 = Engine.Files.LoadEngineShader("Outline2DUnlitForward.fs", EGLSLType.Fragment);
            GLSLScript script3 = Engine.Files.LoadEngineShader("Outline2DUnlitForward.fs", EGLSLType.Fragment);

            TMaterial highlightMat = new TMaterial("OutlineMaterial", new ShaderVar[] 
            {
                new ShaderVec2(Vec2.Zero, "Size"),
                new ShaderFloat(5.0f, "LineWidth"),
                new ShaderVec3((Vec3)Color.Yellow, "Color"),

            }, script1);
            _highlightRC.Mesh = new PrimitiveManager(data1, highlightMat);

            TMaterial selectedMat = new TMaterial("OutlineMaterial", new ShaderVar[] 
            {
                new ShaderVec2(Vec2.Zero, "Size"),
                new ShaderFloat(5.0f, "LineWidth"),
                new ShaderVec3((Vec3)Color.Green, "Color"),

            }, script2);
            _selectedRC.Mesh = new PrimitiveManager(data2, selectedMat);

            TMaterial boundsMat = new TMaterial("OutlineMaterial", new ShaderVar[] 
            {
                new ShaderVec2(Vec2.Zero, "Size"),
                new ShaderFloat(5.0f, "LineWidth"),
                new ShaderVec3(Vec3.Half, "Color"),
            }, script3);
            _uiBoundsRC.Mesh = new PrimitiveManager(data3, boundsMat);

            BaseTransformComponent.WorldTransformChanged += BaseTransformComponent_WorldTransformChanged;
        }

        private void BaseTransformComponent_WorldTransformChanged(TheraEngine.Components.SceneComponent obj)
        {
            if (_targetHud?.RootComponent is UICanvasComponent canvas)
            {
                canvas.Size = _previewResolution;
                _uiBoundsRC.Mesh.Material.Parameter<ShaderVec2>(0).Value = canvas.Size * BaseTransformComponent.Scale;
            }
            if (_highlightedComp is UIBoundableComponent highlightedBounds)
                _highlightRC.Mesh.Material.Parameter<ShaderVec2>(0).Value = highlightedBounds.Size * BaseTransformComponent.Scale;
            if (_selectedComp is UIBoundableComponent selectedBounds)
                _selectedRC.Mesh.Material.Parameter<ShaderVec2>(0).Value = selectedBounds.Size * BaseTransformComponent.Scale;
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
                    _uiBoundsRC.Mesh.Material.Parameter<ShaderVec2>(0).Value = canvas.Size * BaseTransformComponent.Scale;
                }
            }
        }

        public event DelUIComponentSelect UIComponentSelected;
        //public event DelUIComponentSelect UIComponentHighlighted;
        
        public IUserInterface TargetUI
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
                    if (_targetHud?.RootComponent is UICanvasComponent canvas)
                    {
                        canvas.Size = PreviewResolution;
                        _uiBoundsRC.Mesh.Material.Parameter<ShaderVec2>(0).Value = canvas.Size * BaseTransformComponent.Scale;
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
                if (_selectedComp == null)
                    Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = TargetUI;
            }
            else
            {
                _selectedComp = _highlightedComp;
                if (_selectedComp is UIBoundableComponent selectedBounds)
                    _selectedRC.Mesh.Material.Parameter<ShaderVec2>(0).Value = selectedBounds.Size * BaseTransformComponent.Scale;
                _dragComp = null;

                Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = _selectedComp as object ?? TargetUI;
            }
            UIComponentSelected?.Invoke(_dragComp);
        }
        protected override void OnLeftClickUp()
        {
            _dragComp = null;
        }

        private RenderCommandMesh2D _selectedRC = new RenderCommandMesh2D(ERenderPass.TransparentForward);
        private RenderCommandMesh2D _highlightRC = new RenderCommandMesh2D(ERenderPass.TransparentForward);
        private RenderCommandMesh2D _uiBoundsRC = new RenderCommandMesh2D(ERenderPass.TransparentForward);

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
                //Engine.PrintLine(target?.Name ?? "Nothing selected");
                _highlightedComp = target;
                if (_highlightedComp is UIBoundableComponent bound)
                    _highlightRC.Mesh.Material.Parameter<ShaderVec2>(0).Value = bound.Size * BaseTransformComponent.Scale;
            }
            //UIComponentHighlighted?.Invoke(_highlightedComp);
        }

        protected override void HandleDragItem()
        {
            Vec2 diff = GetWorldCursorDiff(CursorPosition());
            _dragComp.LocalTranslation += _dragComp.ScreenToLocal(diff, true);
        }
        protected override void AddRenderables(RenderPasses passes)
        {
            if (_selectedComp != null)
            {
                Matrix4 mtx = _selectedComp.WorldMatrix;
                if (_selectedComp is UIBoundableComponent bound)
                    mtx = mtx * Matrix4.CreateScale(bound.Width, bound.Height, 0.0f);

                _selectedRC.WorldMatrix = mtx;
                passes.Add(_selectedRC);
            }
            if (_highlightedComp != null)
            {
                Matrix4 mtx = _highlightedComp.WorldMatrix;
                if (_highlightedComp is UIBoundableComponent bound)
                    mtx = mtx * Matrix4.CreateScale(bound.Width, bound.Height, 0.0f);

                _highlightRC.WorldMatrix = mtx;
                passes.Add(_highlightRC);
            }

            UICanvasComponent canvas = _targetHud?.RootComponent as UICanvasComponent;
            _uiBoundsRC.WorldMatrix = canvas.WorldMatrix * Matrix4.CreateScale(canvas.Size);
            
            passes.Add(_uiBoundsRC);
        }
    }
}
