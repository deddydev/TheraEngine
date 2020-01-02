using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using TheraEngine;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models.Materials.Functions;
using TheraEngine.Rendering.UI;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEditor.Windows.Forms
{
    public delegate void DelSelectedFunctionChanged(MaterialFunction func);
    /// <summary>
    /// UI editor to create shaders in a user-friendly visual graph format.
    /// </summary>
    public class MaterialEditorUI : EditorUI2DBase, I2DRenderable
    {
        public MaterialEditorUI() : base() { }
        public MaterialEditorUI(Vec2 bounds) : base(bounds) { }
        
        public event DelSelectedFunctionChanged SelectedFunctionChanged;
        
        public TMaterial TargetMaterial
        {
            get => _targetMaterial;
            set
            {
                if (_targetMaterial == value)
                    return;
                _targetMaterial = value;
                if (_targetMaterial != null)
                {
                    if (_targetMaterial.EditorMaterialEnd is null)
                        _targetMaterial.EditorMaterialEnd = Decompile(_targetMaterial);
                    EndFunc = _targetMaterial.EditorMaterialEnd;
                }
                else
                    EndFunc = null;
            }
        }
        private ResultFunc EndFunc
        {
            get => _endFunc;
            set
            {
                if (_endFunc != null)
                    RemoveMaterialFunction(_endFunc);
                _endFunc = value;
                if (_endFunc != null)
                    AddMaterialFunction(_endFunc);
            }
        }
        
        private TMaterial _targetMaterial;
        private ResultFunc _endFunc;
        private MaterialFunction _selectedFunc = null;
        internal MaterialFunction _highlightedFunc = null;
        private MaterialFunction _draggedFunc = null;
        private BaseFuncArg _selectedArg = null;
        private BaseFuncArg _highlightedArg = null;
        private BaseFuncArg _draggedArg = null;
        private List<MaterialFunction> _materialFuncCache = new List<MaterialFunction>();

        private ResultFunc Decompile(TMaterial mat)
        {
            return new ResultPBRFunc() { Material = _targetMaterial };
        }

        public void AddMaterialFunction(MaterialFunction func)
        {
            if (func is null)
                return;

            OriginTransformComponent.ChildComponents.Add(func);
            _materialFuncCache.Add(func);
        }
        public void RemoveMaterialFunction(MaterialFunction func)
        {
            if (func is null)
                return;

            func.DetachFromParent();
            _materialFuncCache.Remove(func);
        }
        
        private void OnArgumentsDisconnected(IFuncValueInput input, IFuncValueOutput output)
            => GenerateShaders();
        private void OnArgumentsConnected(IFuncValueInput input, IFuncValueOutput output)
            => GenerateShaders();
        private void GenerateShaders()
        {
            if (!EndFunc.Generate(out GLSLScript[] shaderFiles, out ShaderVar[] shaderVars))
                return;
            TargetMaterial.Parameters = shaderVars;
            TargetMaterial.Shaders.Set(shaderFiles.Select(x => new GlobalFileRef<GLSLScript>(x)));
        }

        #region Input
        protected override void OnLeftClickDown()
        {
            if (_selectedFunc != null && _selectedFunc != _highlightedFunc)
                _selectedFunc.InterfaceMaterial.Parameter<ShaderVec4>(0).Value = BaseFunction.RegularColor;

            _selectedFunc = _highlightedFunc;
            _selectedArg = _highlightedArg;
            _lastWorldPos = CursorPositionWorld();

            if (_selectedArg != null)
            {
                _draggedArg = _selectedArg;
                if (_draggedArg.IsOutput)
                {
                    if (CtrlDown && _draggedArg is IFuncValueOutput output)
                    {
                        foreach (IFuncValueInput input in output.Connections)
                            OnArgumentsDisconnected(input, output);
                        output.ClearConnections();
                    }

                    UpdateCursorBezier(_draggedArg.WorldPoint.Xy, _lastWorldPos - BoxDim());
                }
                else
                {
                    //if (_draggedArg is IBaseFuncExec exec)
                    //{
                    //    exec.ClearConnection();
                    //}
                    //else 
                    if (_draggedArg is IFuncValueInput input)
                    {
                        OnArgumentsDisconnected(input, input.Connection);
                        input.ClearConnection();
                    }
                    UpdateCursorBezier(_lastWorldPos - BoxDim(), _draggedArg.WorldPoint.Xy);
                }
            }
            else
            {
                _draggedFunc = _selectedFunc;
            }

            SelectedFunctionChanged?.Invoke(_selectedFunc);
        }
        protected override void OnLeftClickUp()
        {
            if (_draggedArg != null && _highlightedArg != null &&
                !ReferenceEquals(_draggedArg, _highlightedArg) &&
                _draggedArg.ConnectTo(_highlightedArg))
            {
                if (_draggedArg is IFuncValueInput input1 && _highlightedArg is IFuncValueOutput output1)
                    OnArgumentsConnected(input1, output1);
                else if (_highlightedArg is IFuncValueInput input2 && _draggedArg is IFuncValueOutput output2)
                    OnArgumentsConnected(input2, output2);
            }

            _draggedFunc = null;
            _inputTree = null;
            _draggedArg = null;
        }

        #region Dragging
        private void HandleDragArg()
        {
            Vec2 posW = CursorPositionWorld();
            if (_draggedArg.IsOutput)
                UpdateCursorBezier(_draggedArg.WorldPoint.Xy, posW - BoxDim());
            else
                UpdateCursorBezier(posW - BoxDim(), _draggedArg.WorldPoint.Xy);
        }
        private void HandleDragFunc()
        {
            Vec2 diff = GetWorldCursorDiff(CursorPosition());
            if (CtrlDown)
            {
                if (_inputTree is null)
                {
                    _inputTree = new HashSet<MaterialFunction>();
                    _draggedFunc.CollectInputTreeRecursive(_inputTree);
                }
                DragInputs(diff);
            }
            else
                _draggedFunc.Translation.Raw += Vec3.TransformVector(diff, _draggedFunc.InverseWorldMatrix).Xy;
        }
        private HashSet<MaterialFunction> _inputTree = null;
        private void DragInputs(Vec2 diff)
        {
            foreach (MaterialFunction f in _inputTree)
                f.Translation.Raw += Vec3.TransformVector(diff, _draggedFunc.InverseWorldMatrix).Xy;
        }
        #endregion

        protected override void HighlightScene()
        {
            IUIComponent comp = FindComponent();

            if (comp is UITextRasterComponent)
                comp = (IUIComponent)comp.ParentSocket;

            if (_highlightedFunc != null && comp != _highlightedFunc)
                _highlightedFunc.InterfaceMaterial.Parameter<ShaderVec4>(0).Value = _highlightedFunc == _selectedFunc ? BaseFunction.SelectedColor : BaseFunction.RegularColor;
            
            if (_highlightedArg != null && comp != _highlightedArg)
            {
                if (_highlightedArg is BaseFuncValue value)
                {
                    _highlightedArg.InterfaceMaterial.Parameter<ShaderVec4>(0).Value = value.GetTypeColor();
                }
                else
                {
                    _highlightedArg.InterfaceMaterial.Parameter<ShaderVec4>(0).Value = BaseFuncExec.DefaultColor;
                }
                UIMaterialRectangleComponent r = _highlightedArg.ParentSocket as UIMaterialRectangleComponent;
                r.InterfaceMaterial.Parameter<ShaderVec4>(0).Value = BaseFunction.RegularColor;
            }

            _highlightedFunc = comp as MaterialFunction;
            _highlightedArg = comp as BaseFuncArg;

            if (_highlightedFunc != null)
                _highlightedFunc.InterfaceMaterial.Parameter<ShaderVec4>(0).Value = BaseFunction.HighlightedColor;

            if (_highlightedArg != null)
            {
                UIMaterialRectangleComponent r = _highlightedArg.ParentSocket as UIMaterialRectangleComponent;
                r.InterfaceMaterial.Parameter<ShaderVec4>(0).Value = BaseFunction.HighlightedColor;

                if (_draggedArg != null &&
                    !ReferenceEquals(_draggedArg, _highlightedArg) &&
                    _draggedArg.CanConnectTo(_highlightedArg))
                {
                    if (_draggedArg.IsOutput)
                        UpdateCursorBezier(_draggedArg.WorldPoint.Xy, _highlightedArg.WorldPoint.Xy);
                    else
                        UpdateCursorBezier(_highlightedArg.WorldPoint.Xy, _draggedArg.WorldPoint.Xy);
                    
                    _highlightedArg.InterfaceMaterial.Parameter<ShaderVec4>(0).Value = BaseFuncValue.ConnectableColor;
                }
                else
                {
                    Vec4 value = _highlightedArg.InterfaceMaterial.Parameter<ShaderVec4>(0).Value;
                    value = new Vec4(0.8f);
                    _highlightedArg.InterfaceMaterial.Parameter<ShaderVec4>(0).Value = value;
                }
            }
        }
        #endregion

        #region Bezier Rendering
        private Segment _cursorBezier = new Segment();
        private void UpdateCursorBezier(Vec2 start, Vec2 end)
        {
            _cursorBezier.StartPoint = start;
            _cursorBezier.EndPoint = end;
        }
        public float BoxDim() => (BaseFuncArg.ConnectionBoxDims * 0.5f) * OriginTransformComponent.Scale.X;
        public float BezierTangentDist { get; set; } = 100.0f;

        protected override bool IsDragging => _draggedArg != null || _draggedFunc != null;
        protected override void HandleDragItem()
        {
            if (_draggedArg != null)
                HandleDragArg();
            else if (_draggedFunc != null)
                HandleDragFunc();
        }
        protected override bool GetWorkArea(out Vec2 min, out Vec2 max)
        {
            if (EndFunc is null)
            {
                min = Vec2.Zero;
                max = Vec2.Zero;
                return false;
            }
            EndFunc.GetMinMax(out min, out max);
            return true;
        }
        protected override void RenderMethod()
        {
            float boxDim = BoxDim();
            foreach (MaterialFunction m in _materialFuncCache)
                foreach (var input in m.InputArguments)
                    if (input.Connection != null)
                        DrawBezier(input.Connection.WorldPoint.Xy + boxDim, input.WorldPoint.Xy + boxDim, input.GetTypeColor());

            if (_draggedArg is null)
                return;

            Vec2 start = _cursorBezier.StartPoint.Xy + boxDim;
            Vec2 end = _cursorBezier.EndPoint.Xy + boxDim;
            DrawBezier(start, end, BaseFuncValue.NoTypeColor);

            base.RenderMethod();
        }
        private void DrawBezier(Vec2 start, Vec2 end, ColorF4 color)
        {
            float dist = start.DistanceToFast(end).ClampMax(BezierTangentDist);
            Vec2 diff = new Vec2(dist * OriginTransformComponent.Scale.X, 0.0f);
            Vec2[] points = Interp.GetBezierPoints(start, start + diff, end - diff, end, 20);
            for (int i = 1; i < points.Length; ++i)
                Engine.Renderer.RenderLine(points[i - 1], points[i], color, true, 1.0f);
        }

        //protected override void AddRenderables(RenderPasses passes) => base.AddRenderables(passes);
        #endregion
    }
}
