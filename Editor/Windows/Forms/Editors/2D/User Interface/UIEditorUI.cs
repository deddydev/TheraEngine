using System;
using System.Drawing;
using TheraEngine;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Components;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.UI;

namespace TheraEditor.Windows.Forms
{
    public delegate void DelUIComponentSelect(IUIComponent comp);
    /// <summary>
    /// UI for editing UIs. How meta.
    /// </summary>
    public class UIEditorUI : EditorUI2DBase, I2DRenderable
    {
        public UIEditorUI() : base()
        {
            VertexQuad quad = VertexQuad.PosZQuad(1, true, -0.5f, false);
            VertexTriangle[] lines = quad.ToTriangles();

            PrimitiveData data1 = PrimitiveData.FromTriangles(VertexShaderDesc.PosTex(), lines);
            PrimitiveData data2 = PrimitiveData.FromTriangles(VertexShaderDesc.PosTex(), lines);
            PrimitiveData data3 = PrimitiveData.FromTriangles(VertexShaderDesc.PosTex(), lines);

            GLSLScript script1 = Engine.Files.Shader("Outline2DUnlitForward.fs", EGLSLType.Fragment);
            GLSLScript script2 = Engine.Files.Shader("Outline2DUnlitForward.fs", EGLSLType.Fragment);
            GLSLScript script3 = Engine.Files.Shader("Outline2DUnlitForward.fs", EGLSLType.Fragment);

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

            ContextMenu = new TMenuComponent();
            ContextMenu.ChildComponents.Add(new TMenuItemComponent());
        }
        protected override void ResizeLayout()
        {
            base.ResizeLayout();

            if (_targetHud?.RootComponent is UICanvasComponent canvas)
                _uiBoundsRC.Mesh.Material.Parameter<ShaderVec2>(0).Value = canvas.ActualSize.Raw * canvas.WorldMatrix.Scale.Xy;
            
            if (_highlightedComp is UIBoundableComponent highlightedBounds)
                _highlightRC.Mesh.Material.Parameter<ShaderVec2>(0).Value = highlightedBounds.ActualSize.Raw * highlightedBounds.WorldMatrix.Scale.Xy;

            if (_selectedComp is UIBoundableComponent selectedBounds)
                _selectedRC.Mesh.Material.Parameter<ShaderVec2>(0).Value = selectedBounds.ActualSize.Raw * selectedBounds.WorldMatrix.Scale.Xy;
        }

        private Vec2 _previewResolution = new Vec2(1920, 1080);
        public Vec2 PreviewResolution
        {
            get => _previewResolution;
            set
            {
                _previewResolution = value;
                if (_targetHud?.RootComponent is IUICanvasComponent canvas)
                    canvas.Size.Raw = _previewResolution;
            }
        }

        public event DelUIComponentSelect UIComponentSelected;
        //public event DelUIComponentSelect UIComponentHighlighted;
        
        public IUserInterfacePawn TargetUI
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
                    if (_targetHud?.RootComponent is IUICanvasComponent canvas)
                        InitCanvas(canvas);
                }
                else
                {

                }
            }
        }

        private void InitCanvas(IUICanvasComponent canvas)
        {
            canvas.HorizontalAlignment = EHorizontalAlign.Positional;
            canvas.VerticalAlignment = EVerticalAlign.Positional;
            canvas.Size.Raw = PreviewResolution;
            canvas.Translation.Raw = Vec3.Zero;
            canvas.Scale.Raw = Vec3.One;
        }

        protected override bool IsDragging => _dragComp != null;

        private UITransformComponent _dragComp;
        private UITransformComponent _selectedComp, _highlightedComp;

        private IUserInterfacePawn _targetHud;
        
        //public override void RegisterInput(InputInterface input)
        //{
        //    base.RegisterInput(input);
        //}
        protected override void OnLeftClickDown()
        {
            var cursorPos = CursorPosition();
            if (!Bounds.Raw.Contains(cursorPos))
                return;

            if (_selectedComp == _highlightedComp)
            {
                _dragComp = _selectedComp;
                if (_selectedComp is null)
                    Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = TargetUI;
            }
            else
            {
                _selectedComp = _highlightedComp;
                if (_selectedComp is UIBoundableComponent selectedBounds)
                    _selectedRC.Mesh.Material.Parameter<ShaderVec2>(0).Value = selectedBounds.ActualSize.Raw * selectedBounds.WorldMatrix.Scale.Xy;
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

        protected override bool GetWorkArea(out Vec2 min, out Vec2 max)
        {
            if (_selectedComp != null)
            {
                min = Vec3.TransformPosition(_selectedComp.RenderInfo.AxisAlignedRegion.Min, BaseTransformComponent.InverseLocalMatrix).Xy;
                max = Vec3.TransformPosition(_selectedComp.RenderInfo.AxisAlignedRegion.Max, BaseTransformComponent.InverseLocalMatrix).Xy;
            }
            else
            {
                min = Vec2.Zero;
                max = PreviewResolution;
            }
            return true;
        }
        protected override void HighlightScene()
        {
            if (IsDragging)
                return;

            IUIComponent target = (_targetHud?.RootComponent as UICanvasComponent)?.FindDeepestComponent(CursorPositionWorld(), false);
            if (target != _highlightedComp)
            {
                //Engine.PrintLine(target?.Name ?? "Nothing selected");

                if (target is UITransformComponent tc)
                _highlightedComp = tc;

                if (_highlightedComp is UIBoundableComponent bound)
                    _highlightRC.Mesh.Material.Parameter<ShaderVec2>(0).Value = bound.ActualSize.Raw * bound.WorldMatrix.Scale.Xy;
            }
            //UIComponentHighlighted?.Invoke(_highlightedComp);
        }

        protected override void HandleDragItem()
        {
            Vec2 diff = GetWorldCursorDiff(CursorPosition());
            _dragComp.Translation.Raw += _dragComp.ScreenToLocal(diff, true);
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

            if (_targetHud?.RootComponent is UICanvasComponent canvas)
                _uiBoundsRC.WorldMatrix = canvas.WorldMatrix * Matrix4.CreateScale(canvas.ActualSize.Raw);

            passes.Add(_uiBoundsRC);
        }
    }
}
