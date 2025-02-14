﻿using System;
using System.Drawing;
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
    public delegate void DelUIComponentSelect(IUIComponent comp);
    /// <summary>
    /// UI for editing UIs. How meta.
    /// </summary>
    public class UIEditorUI : EditorUI2DBase, I2DRenderable
    {
        public UIEditorUI() : base()
        {
            TVertexQuad quad = TVertexQuad.PosZ(1, true, -0.5f, false);
            TVertexTriangle[] tris = quad.ToTriangles();

            TMesh data1 = TMesh.Create(VertexShaderDesc.PosTex(), tris);
            TMesh data2 = TMesh.Create(VertexShaderDesc.PosTex(), tris);
            TMesh data3 = TMesh.Create(VertexShaderDesc.PosTex(), tris);

            GLSLScript script1 = Engine.Files.Shader("Outline2DUnlitForward.fs", EGLSLType.Fragment);
            GLSLScript script2 = Engine.Files.Shader("Outline2DUnlitForward.fs", EGLSLType.Fragment);
            GLSLScript script3 = Engine.Files.Shader("Outline2DUnlitForward.fs", EGLSLType.Fragment);

            TMaterial highlightMat = new TMaterial("OutlineMaterial", new ShaderVar[]
            {
                new ShaderVec2(Vec2.Zero, "Size"),
                new ShaderFloat(5.0f, "LineWidth"),
                new ShaderVec3((Vec3)Color.Yellow, "Color"),

            }, script1);
            _highlightRC.Mesh = new MeshRenderer(data1, highlightMat);

            TMaterial selectedMat = new TMaterial("OutlineMaterial", new ShaderVar[]
            {
                new ShaderVec2(Vec2.Zero, "Size"),
                new ShaderFloat(5.0f, "LineWidth"),
                new ShaderVec3((Vec3)Color.Green, "Color"),

            }, script2);
            _selectedRC.Mesh = new MeshRenderer(data2, selectedMat);

            TMaterial boundsMat = new TMaterial("OutlineMaterial", new ShaderVar[]
            {
                new ShaderVec2(Vec2.Zero, "Size"),
                new ShaderFloat(5.0f, "LineWidth"),
                new ShaderVec3(Vec3.Half, "Color"),
            }, script3);
            _uiBoundsRC.Mesh = new MeshRenderer(data3, boundsMat);

            ContextMenu = new TMenuComponent();
            ContextMenu.ChildSockets.Add(new TMenuItemComponent());
        }
        public override void RegisterInput(InputInterface input)
        {
            base.RegisterInput(input);
            input.RegisterKeyEvent(EKey.Delete, EButtonInputType.Pressed, OnDelete, EInputPauseType.TickAlways);
        }

        private void OnDelete()
        {
            _selectedComp?.RemoveSelf();
        }

        protected override void ResizeLayout()
        {
            base.ResizeLayout();

            if (_targetHud?.RootComponent is UICanvasComponent canvas)
                _uiBoundsRC.Mesh.Material.Parameter<ShaderVec2>(0).Value = canvas.ActualSize.Value * canvas.WorldMatrix.Value.Scale.Xy;
            
            if (_highlightedComp is UIBoundableComponent highlightedBounds)
                _highlightRC.Mesh.Material.Parameter<ShaderVec2>(0).Value = highlightedBounds.ActualSize.Value * highlightedBounds.WorldMatrix.Value.Scale.Xy;

            if (_selectedComp is UIBoundableComponent selectedBounds)
                _selectedRC.Mesh.Material.Parameter<ShaderVec2>(0).Value = selectedBounds.ActualSize.Value * selectedBounds.WorldMatrix.Value.Scale.Xy;
        }

        private Vec2 _designSize = new Vec2(1920, 1080);
        public Vec2 DesignSize
        {
            get => _designSize;
            set
            {
                _designSize = value;
                if (_targetHud?.RootComponent is IUICanvasComponent canvas)
                    canvas.Size.Value = _designSize;
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
                    OriginTransformComponent.ChildSockets.Remove(_targetHud.RootComponent);
                }
                _targetHud = value;
                if (_targetHud != null)
                {
                    OriginTransformComponent.ChildSockets.Add(_targetHud.RootComponent);
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
            canvas.Size.Value = DesignSize;
            canvas.Translation.Value = Vec3.Zero;
            canvas.Scale.Value = Vec3.One;
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
            if (!Bounds.Value.Contains(cursorPos))
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
                    _selectedRC.Mesh.Material.Parameter<ShaderVec2>(0).Value = selectedBounds.ActualSize.Value * selectedBounds.WorldMatrix.Value.Scale.Xy;
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
                min = Vec3.TransformPosition(_selectedComp.RenderInfo2D.AxisAlignedRegion.Min, OriginTransformComponent.InverseLocalMatrix.Value).Xy;
                max = Vec3.TransformPosition(_selectedComp.RenderInfo2D.AxisAlignedRegion.Max, OriginTransformComponent.InverseLocalMatrix.Value).Xy;
            }
            else
            {
                min = Vec2.Zero;
                max = DesignSize;
            }
            return true;
        }
        protected override void HighlightScene()
        {
            if (IsDragging)
                return;

            UICanvasComponent canvas = _targetHud?.RootComponent as UICanvasComponent;
            Vec2 pos = CursorPositionWorld();
            IUIComponent target = canvas?.FindDeepestComponent(pos, false);
            if (target != _highlightedComp)
            {
                //Engine.PrintLine(target?.Name ?? "Nothing selected");

                if (target is UITransformComponent tc)
                    _highlightedComp = tc;
                else
                    _highlightedComp = null;

                if (_highlightedComp is UIBoundableComponent bound)
                    _highlightRC.Mesh.Material.Parameter<ShaderVec2>(0).Value = bound.ActualSize.Value * bound.WorldMatrix.Value.Scale.Xy;
            }
            //UIComponentHighlighted?.Invoke(_highlightedComp);
        }

        protected override void HandleDragItem()
        {
            Vec2 pos = CursorPosition();
            Vec2 diff = GetWorldCursorDiff(pos);
            _dragComp.Translation.Value += _dragComp.ScreenToLocal(diff, true);
        }
        protected override void AddRenderables(RenderPasses passes)
        {
            if (_selectedComp != null)
            {
                Matrix4 mtx = _selectedComp.WorldMatrix.Value;
                if (_selectedComp is UIBoundableComponent bound)
                    mtx = mtx * Matrix4.CreateScale(bound.ActualWidth, bound.ActualHeight, 0.0f);

                _selectedRC.WorldMatrix = mtx;
                passes.Add(_selectedRC);
            }
            if (_highlightedComp != null)
            {
                Matrix4 mtx = _highlightedComp.WorldMatrix.Value;
                if (_highlightedComp is UIBoundableComponent bound)
                    mtx = mtx * Matrix4.CreateScale(bound.ActualWidth, bound.ActualHeight, 0.0f);

                _highlightRC.WorldMatrix = mtx;
                passes.Add(_highlightRC);
            }

            if (_targetHud?.RootComponent is UICanvasComponent canvas)
                _uiBoundsRC.WorldMatrix = canvas.WorldMatrix.Value * Matrix4.CreateScale(canvas.ActualSize.Value);

            passes.Add(_uiBoundsRC);
        }
    }
}
