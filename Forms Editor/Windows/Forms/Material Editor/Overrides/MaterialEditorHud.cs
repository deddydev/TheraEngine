using System;
using TheraEngine;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models.Materials.Functions;
using TheraEngine.Rendering.UI;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEditor.Windows.Forms
{
    /// <summary>
    /// UI editor to create shaders in a user-friendly visual graph format.
    /// </summary>
    public class UIMaterialEditor : UIManager<UIMaterialRectangleComponent>
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
                    _rootTransform.ChildComponents.Remove(_endFunc);
                _endFunc = value;
                if (_endFunc != null)
                    _rootTransform.ChildComponents.Add(_endFunc);
            }
        }

        private TMaterial _targetMaterial;
        private ResultFunc _endFunc;
        private Vec2 _minScale = new Vec2(0.1f), _maxScale = new Vec2(4.0f);
        private Vec2 _lastWorldPos = Vec2.Zero;
        //private Vec2 _lastFocusPoint = Vec2.Zero;
        private MaterialFunction _selectedFunc = null;
        private MaterialFunction _highlightedFunc = null;
        private MaterialFunction _draggedFunc = null;
        private BaseFuncArg _selectedArg = null;
        private BaseFuncArg _highlightedArg = null;
        private BaseFuncArg _draggedArg = null;
        private UIComponent _rootTransform;
        bool _rightClickDown = false;

        private UIComponent FindComponent()
            => FindComponent(CursorPositionWorld());
        private UIComponent FindComponent(Vec2 cursorWorldPos)
            => RootComponent.FindComponent(cursorWorldPos);

        #region Input
        public override void RegisterInput(InputInterface input)
        {
            input.RegisterButtonEvent(EMouseButton.LeftClick, ButtonInputType.Pressed, LeftClickDown, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick, ButtonInputType.Released, LeftClickUp, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.RightClick, ButtonInputType.Pressed, RightClickDown, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.RightClick, ButtonInputType.Released, RightClickUp, EInputPauseType.TickAlways);
            input.RegisterMouseScroll(OnScrolledInput, EInputPauseType.TickAlways);
            input.RegisterMouseMove(MouseMove, false, EInputPauseType.TickAlways);
        }
        private void LeftClickDown()
        {
            _selectedFunc = _draggedFunc = _highlightedFunc;
            _selectedArg = _draggedArg = _highlightedArg;
            _lastWorldPos = CursorPositionWorld();
        }
        private void LeftClickUp()
        {
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
                UpdateCursorBezier(_draggedArg.WorldPoint.Xy, posW);
            }
            else if (_rightClickDown)
            {
                Vec2 screenPoint = Viewport.WorldToScreen(_lastWorldPos).Xy;
                screenPoint += pos - _cursorPos;
                Vec2 newFocusPoint = Viewport.ScreenToWorld(screenPoint).Xy;
                _rootTransform.LocalTranslation += newFocusPoint - _lastWorldPos;
                _lastWorldPos = newFocusPoint;
            }
            else
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
                    _highlightedArg.InterfaceMaterial.Parameter<ShaderVec4>(0).Value = BaseFuncArg.HighlightedColor;
                    UIMaterialRectangleComponent r = _highlightedArg.ParentSocket as UIMaterialRectangleComponent;
                    r.InterfaceMaterial.Parameter<ShaderVec4>(0).Value = BaseFunction.HighlightedColor;
                }
            }
            _cursorPos = pos;
        }
        private void UpdateCursorBezier(Vec2 start, Vec2 end)
        {

        }
        protected override void OnScrolledInput(bool down)
        {
            Vec3 worldPoint = CursorPositionWorld();
            _rootTransform.Zoom(down ? 0.1f : -0.1f, worldPoint.Xy, _minScale, _maxScale);
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
            //root.ChildComponents.Add(_bezierRect);
            return root;
        }
        private TMaterial GetGraphMaterial()
        {
            Shader frag = Engine.LoadEngineShader("MaterialEditorGraphBG.fs", ShaderMode.Fragment);
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
    }
}
