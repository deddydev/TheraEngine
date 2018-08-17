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
    public delegate void DelSelectedFunctionChanged(MaterialFunction func);
    /// <summary>
    /// UI editor to create shaders in a user-friendly visual graph format.
    /// </summary>
    public class UIMaterialEditor : UIManager<UIMaterialRectangleComponent>, I2DRenderable
    {
        public UIMaterialEditor() : base() { }
        public UIMaterialEditor(Vec2 bounds) : base(bounds) { }

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
                    if (_targetMaterial.EditorMaterialEnd == null)
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

        RenderInfo2D I2DRenderable.RenderInfo { get; } = new RenderInfo2D(ERenderPass.OnTopForward, 0, 0);
        public BoundingRectangleF AxisAlignedRegion { get; } = new BoundingRectangleF();
        public IQuadtreeNode QuadtreeNode { get; set; }

        private TMaterial _targetMaterial;
        private ResultFunc _endFunc;
        private Vec2 _minScale = new Vec2(0.1f), _maxScale = new Vec2(4.0f);
        private Vec2 _lastWorldPos = Vec2.Zero;
        //private Vec2 _lastFocusPoint = Vec2.Zero;
        private MaterialFunction _selectedFunc = null;
        internal MaterialFunction _highlightedFunc = null;
        private MaterialFunction _draggedFunc = null;
        private BaseFuncArg _selectedArg = null;
        private BaseFuncArg _highlightedArg = null;
        private BaseFuncArg _draggedArg = null;
        internal UIComponent _rootTransform;
        private bool _rightClickDown = false;
        private List<MaterialFunction> _materialFuncCache = new List<MaterialFunction>();
        
        public void AddMaterialFunction(MaterialFunction func)
        {
            if (func == null)
                return;

            _rootTransform.ChildComponents.Add(func);
            _materialFuncCache.Add(func);
        }
        public void RemoveMaterialFunction(MaterialFunction func)
        {
            if (func == null)
                return;

            func.DetachFromParent();
            _materialFuncCache.Remove(func);
        }

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

        private void OnArgumentsDisconnected(IFuncValueInput input, IFuncValueOutput output)
            => GenerateShaders();
        private void OnArgumentsConnected(IFuncValueInput input, IFuncValueOutput output)
            => GenerateShaders();
        private void GenerateShaders()
        {
            if (!EndFunc.Generate(out GLSLShaderFile[] shaderFiles, out ShaderVar[] shaderVars))
                return;
            TargetMaterial.Parameters = shaderVars;
            TargetMaterial.Shaders.Set(shaderFiles.Select(x => new GlobalFileRef<GLSLShaderFile>(x)));
        }

        private ResultFunc Decompile(TMaterial mat)
        {
            return new ResultPBRFunc() { Material = _targetMaterial };
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
                    if (_altDown && _draggedArg is IFuncValueOutput output)
                    {
                        foreach (IFuncValueInput input in output)
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
        internal void LeftClickUp()
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
            if (_draggedArg != null)
                HandleDragArg(pos, _draggedArg);
            else if (_draggedFunc != null)
                HandleDragFunc(pos, _draggedFunc);
            else if (_rightClickDown)
                HandleDragView(pos);
            else
                HighlightGraph();
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
        private void HandleDragArg(Vec2 cursorPosScreen, BaseFuncArg draggedArg)
        {
            Vec2 posW = Viewport.ScreenToWorld(cursorPosScreen).Xy;
            if (draggedArg.IsOutput)
                UpdateCursorBezier(draggedArg.WorldPoint.Xy, posW - BoxDim());
            else
                UpdateCursorBezier(posW - BoxDim(), draggedArg.WorldPoint.Xy);
            HighlightGraph();
        }
        private void HandleDragView(Vec2 cursorPosScreen)
        {
            _rootTransform.LocalTranslation += GetWorldCursorDiff(cursorPosScreen);

            TMaterial mat = RootComponent.InterfaceMaterial;
            mat.Parameter<ShaderVec2>(4).Value = _rootTransform.LocalTranslation;
        }
        private void HandleDragFunc(Vec2 cursorPosScreen, MaterialFunction draggedFunc)
        {
            Vec2 diff = GetWorldCursorDiff(cursorPosScreen);
            if (_ctrlDown)
            {
                if (_inputTree == null)
                {
                    _inputTree = new HashSet<MaterialFunction>();
                    draggedFunc.CollectInputTreeRecursive(_inputTree);
                }
                DragInputs(diff);
            }
            else
                draggedFunc.LocalTranslation += Vec3.TransformVector(diff, draggedFunc.InverseWorldMatrix).Xy;
        }
        private HashSet<MaterialFunction> _inputTree = null;
        private void DragInputs(Vec2 diff)
        {
            foreach (MaterialFunction f in _inputTree)
                f.LocalTranslation += Vec3.TransformVector(diff, _draggedFunc.InverseWorldMatrix).Xy;
        }
        #endregion

        private void HighlightGraph()
        {
            UIComponent comp = FindComponent();

            if (comp is UITextComponent)
                comp = (UIComponent)comp.ParentSocket;

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
        protected override void OnScrolledInput(bool down)
        {
            Vec3 worldPoint = CursorPositionWorld();
            _rootTransform.Zoom(down ? 0.1f : -0.1f, worldPoint.Xy, _minScale, _maxScale);

            TMaterial mat = RootComponent.InterfaceMaterial;
            mat.Parameter<ShaderFloat>(2).Value = _rootTransform.ScaleX;
            mat.Parameter<ShaderVec2>(4).Value = _rootTransform.LocalTranslation;
        }
        #endregion

        #region Bezier Rendering
        private Segment _cursorBezier = new Segment();
        private void UpdateCursorBezier(Vec2 start, Vec2 end)
        {
            _cursorBezier.StartPoint = start;
            _cursorBezier.EndPoint = end;
        }
        public float BoxDim() => (BaseFuncArg.ConnectionBoxDims * 0.5f) * _rootTransform.ScaleX;
        public void Render()
        {
            float boxDim = BoxDim();
            foreach (MaterialFunction m in _materialFuncCache)
                foreach (var input in m.InputArguments)
                    if (input.Connection != null)
                        DrawBezier(input.Connection.WorldPoint.Xy + boxDim, input.WorldPoint.Xy + boxDim, input.GetTypeColor());

            if (_draggedArg == null)
                return;

            Vec2 start = _cursorBezier.StartPoint.Xy + boxDim;
            Vec2 end = _cursorBezier.EndPoint.Xy + boxDim;
            DrawBezier(start, end, BaseFuncValue.NoTypeColor);
        }
        public float BezierTangentDist { get; set; } = 100.0f;
        private void DrawBezier(Vec2 start, Vec2 end, ColorF4 color)
        {
            float dist = start.DistanceToFast(end).ClampMax(BezierTangentDist);
            Vec2 diff = new Vec2(dist * _rootTransform.ScaleX, 0.0f);
            Vec2[] points = Interp.GetBezierPoints(start, start + diff, end - diff, end, 20);
            for (int i = 1; i < points.Length; ++i)
                Engine.Renderer.RenderLine(points[i - 1], points[i], color, true, 1.0f);
        }

        public void AddRenderables(RenderPasses passes)
        {

        }
        #endregion
    }
}
