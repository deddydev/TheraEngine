using System;
using System.Collections.Generic;
using System.Drawing;
using TheraEngine;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Core.Shapes;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models.Materials.Functions;
using TheraEngine.Rendering.UI;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEditor.Windows.Forms
{
    /// <summary>
    /// UI editor to create shaders in a user-friendly visual graph format.
    /// </summary>
    public class UIMaterialEditor : UIManager<UIMaterialRectangleComponent>, I2DRenderable
    {
        public UIMaterialEditor() : base() { }
        public UIMaterialEditor(Vec2 bounds) : base(bounds) { }

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

        public void RemoveMaterialFunction(MaterialFunction func)
        {
            if (func == null)
                return;

            func.DetachFromParent();
            _materialFuncCache.Remove(func);
        }

        RenderInfo2D I2DRenderable.RenderInfo { get; } = new RenderInfo2D(ERenderPass2D.OnTop, 0, 0);

        public BoundingRectangle AxisAlignedRegion { get; } = new BoundingRectangle();

        public IQuadtreeNode QuadtreeNode { get; set; }

        public void AddMaterialFunction(MaterialFunction func)
        {
            if (func == null)
                return;

            _rootTransform.ChildComponents.Add(func);
            _materialFuncCache.Add(func);
        }

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
        bool _rightClickDown = false;
        private List<MaterialFunction> _materialFuncCache = new List<MaterialFunction>();

        private UIComponent FindComponent()
            => FindComponent(CursorPositionWorld());
        private UIComponent FindComponent(Vec2 cursorWorldPos)
            => RootComponent.FindDeepestComponent(cursorWorldPos);

        public override void OnSpawnedPostComponentSetup()
        {
            base.OnSpawnedPostComponentSetup();
            UIScene.Add(this);
        }
        public override void OnDespawned()
        {
            base.OnDespawned();
            UIScene.Remove(this);
        }

        #region Input
        public override void RegisterInput(InputInterface input)
        {
            input.RegisterButtonPressed(EKey.AltLeft, b => _altDown = b, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick, ButtonInputType.Pressed, LeftClickDown, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick, ButtonInputType.Released, LeftClickUp, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.RightClick, ButtonInputType.Pressed, RightClickDown, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.RightClick, ButtonInputType.Released, RightClickUp, EInputPauseType.TickAlways);
            input.RegisterMouseScroll(OnScrolledInput, EInputPauseType.TickAlways);
            input.RegisterMouseMove(MouseMove, MouseMoveType.Absolute, EInputPauseType.TickAlways);
        }
        private bool _altDown = false;
        internal void LeftClickDown()
        {
            _selectedFunc = _draggedFunc = _highlightedFunc;
            _selectedArg = _draggedArg = _highlightedArg;
            _lastWorldPos = CursorPositionWorld();
            if (_draggedArg != null)
            {
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
        }

        private void OnArgumentsDisconnected(IFuncValueInput input, IFuncValueOutput output)
        {
            GenerateShaders();
        }
        private void OnArgumentsConnected(IFuncValueInput input, IFuncValueOutput output)
        {
            GenerateShaders();
        }

        private void GenerateShaders()
        {
            ShaderFile[] shaders = EndFunc.GenerateShaders();
            TargetMaterial.SetShaders(EndFunc.GenerateShaders());
            
        }

        internal void LeftClickUp()
        {
            if (_draggedArg != null && _highlightedArg != null &&
                !ReferenceEquals(_draggedArg, _highlightedArg) &&
                _draggedArg.TryConnectTo(_highlightedArg))
            {
                if (_draggedArg is IFuncValueInput input1 && _highlightedArg is IFuncValueOutput output1)
                    OnArgumentsConnected(input1, output1);
                else if (_highlightedArg is IFuncValueInput input2 && _draggedArg is IFuncValueOutput output2)
                    OnArgumentsConnected(input2, output2);
            }

            _draggedFunc = null;
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
            if (_draggedFunc != null)
            {
                Vec2 screenPoint = Viewport.WorldToScreen(_lastWorldPos).Xy;
                screenPoint += pos - _cursorPos;
                Vec2 newFocusPoint = Viewport.ScreenToWorld(screenPoint).Xy;
                _draggedFunc.LocalTranslation += Vec3.TransformVector((newFocusPoint - _lastWorldPos), _draggedFunc.InverseWorldMatrix).Xy;
                _lastWorldPos = newFocusPoint;
            }
            else if (_draggedArg != null)
            {
                Vec2 posW = Viewport.ScreenToWorld(pos).Xy;
                if (_draggedArg.IsOutput)
                {
                    UpdateCursorBezier(_draggedArg.WorldPoint.Xy, posW - BoxDim());
                }
                else
                {
                    UpdateCursorBezier(posW - BoxDim(), _draggedArg.WorldPoint.Xy);
                }
                RegularHighlight();
            }
            else if (_rightClickDown)
            {
                Vec2 screenPoint = Viewport.WorldToScreen(_lastWorldPos).Xy;
                screenPoint += pos - _cursorPos;
                Vec2 newFocusPoint = Viewport.ScreenToWorld(screenPoint).Xy;
                _rootTransform.LocalTranslation += newFocusPoint - _lastWorldPos;
                _lastWorldPos = newFocusPoint;

                TMaterial mat = RootComponent.InterfaceMaterial;
                mat.Parameter<ShaderVec2>(4).Value = _rootTransform.LocalTranslation;
            }
            else
            {
                RegularHighlight();
            }
            _cursorPos = pos;
        }
        private void RegularHighlight()
        {
            UIComponent comp = FindComponent();

            if (comp is UITextComponent)
                comp = (UIComponent)comp.ParentSocket;

            if (_highlightedFunc != null && comp != _highlightedFunc)
                _highlightedFunc.InterfaceMaterial.Parameter<ShaderVec4>(0).Value = BaseFunction.RegularColor;

            if (_highlightedArg != null && comp != _highlightedArg)
            {
                _highlightedArg.InterfaceMaterial.Parameter<ShaderVec4>(0).Value = BaseFuncArg.RegularColor;
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
                    {
                        UpdateCursorBezier(_draggedArg.WorldPoint.Xy, _highlightedArg.WorldPoint.Xy);
                    }
                    else
                    {
                        UpdateCursorBezier(_highlightedArg.WorldPoint.Xy, _draggedArg.WorldPoint.Xy);
                    }
                    _highlightedArg.InterfaceMaterial.Parameter<ShaderVec4>(0).Value = BaseFuncArg.ConnectableColor;
                }
                else
                {
                    _highlightedArg.InterfaceMaterial.Parameter<ShaderVec4>(0).Value = BaseFuncArg.HighlightedColor;
                }
            }
        }
        private Segment _cursorBezier = new Segment();
        private void UpdateCursorBezier(Vec2 start, Vec2 end)
        {
            _cursorBezier.StartPoint = start;
            _cursorBezier.EndPoint = end;

            //_cursorBezier.StartPoint.Xy += BaseFuncArg.ConnectionBoxDims * 0.5f;
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
        private TMaterial GetGraphMaterial()
        {
            ShaderFile frag = Engine.LoadEngineShader("MaterialEditorGraphBG.fs", ShaderMode.Fragment);
            return new TMaterial("MatEditorGraphBG", new ShaderVar[] 
            {
                new ShaderVec3(new Vec3(0.1f, 0.12f, 0.13f), "LineColor"),
                new ShaderVec3(new Vec3(0.25f, 0.27f, 0.3f), "BGColor"),
                new ShaderFloat(1.0f, "Scale"),
                new ShaderFloat(0.15f, "LineWidth"),
                new ShaderVec2(new Vec2(0.0f), "Translation"),
            },
            frag);
        }
        private ResultFunc Decompile(TMaterial mat)
        {
            return new ResultPBRFunc() { Material = _targetMaterial };
        }

        public bool Contains(Vec2 point)
        {
            throw new NotImplementedException();
        }

        public float BoxDim() => (BaseFuncArg.ConnectionBoxDims * 0.5f) * _rootTransform.ScaleX;
        public void Render()
        {
            float boxDim = BoxDim();
            foreach (MaterialFunction m in _materialFuncCache)
                foreach (var input in m.InputArguments)
                    if (input.Connection != null)
                        DrawBezier(input.Connection.WorldPoint.Xy + boxDim, input.WorldPoint.Xy + boxDim);

            if (_draggedArg == null)
                return;

            Vec2 start = _cursorBezier.StartPoint.Xy + boxDim;
            Vec2 end = _cursorBezier.EndPoint.Xy + boxDim;
            DrawBezier(start, end);
        }
        public float BezierTangentDist { get; set; } = 100.0f;
        private void DrawBezier(Vec2 start, Vec2 end)
        {
            Vec2 diff = new Vec2(BezierTangentDist * _rootTransform.ScaleX, 0.0f);
            //if (end.X < start.X)
            //    diff.X = -diff.X;

            Vec2[] points = Interp.GetBezierPoints(start, start + diff, end - diff, end, 20);
            for (int i = 1; i < points.Length; ++i)
                Engine.Renderer.RenderLine(
                    points[i - 1],
                    points[i],
                    Color.Orange, 1.0f);
        }
    }
}
