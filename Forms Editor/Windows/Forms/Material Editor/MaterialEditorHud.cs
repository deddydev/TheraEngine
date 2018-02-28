using System;
using System.Drawing;
using TheraEngine;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models.Materials.Functions;
using TheraEngine.Rendering.UI;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEditor.Windows.Forms
{
    public class UIMaterialEditor : UIManager<UIMaterialRectangleComponent>
    {
        public UIMaterialEditor() : base() { }
        public UIMaterialEditor(Vec2 bounds) : base(bounds) { }

        public override void RegisterInput(InputInterface input)
        {
            input.RegisterButtonEvent(EMouseButton.LeftClick, ButtonInputType.Pressed, LeftClickDown, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick, ButtonInputType.Released, LeftClickUp, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.RightClick, ButtonInputType.Pressed, RightClickDown, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.RightClick, ButtonInputType.Released, RightClickUp, EInputPauseType.TickAlways);
            input.RegisterMouseScroll(OnScrolledInput, EInputPauseType.TickAlways);
            input.RegisterMouseMove(MouseMove, false, EInputPauseType.TickAlways);
        }

        private Vec2 _lastWorldPos = Vec2.Zero;
        //private Vec2 _lastFocusPoint = Vec2.Zero;
        private MaterialFunction _selectedFunc = null;
        private MaterialFunction _highlightedFunc = null;
        private MaterialFunction _draggedFunc = null;
        private BaseFuncArg _selectedArg = null;
        private BaseFuncArg _highlightedArg = null;
        private BaseFuncArg _draggedArg = null;
        private UIComponent _rootTransform;
        private UIMaterialRectangleComponent _bezierRect = new UIMaterialRectangleComponent();
        bool _rightClickDown = false;

        private void LeftClickDown()
        {
            _selectedFunc = _draggedFunc = _highlightedFunc;
            _selectedArg = _draggedArg = _highlightedArg;
            _lastWorldPos = Viewport.ScreenToWorld(Viewport.AbsoluteToRelative(CursorPosition), 0.0f).Xy;
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
            _lastWorldPos = Viewport.ScreenToWorld(Viewport.AbsoluteToRelative(CursorPosition)).Xy;
        }
        private void RightClickUp()
        {
            _rightClickDown = false;
        }
        private UIComponent FindComponent()
        {
            return RootComponent.FindComponent(Viewport.ScreenToWorld(Viewport.AbsoluteToRelative(CursorPosition)).Xy);
        }
        protected override void MouseMove(float x, float y)
        {
            Vec2 pos = Viewport.AbsoluteToRelative(CursorPosition);
            
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

                if (comp is TextHudComponent)
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

        private Vec2 _minScale = new Vec2(0.05f), _maxScale = new Vec2(3.0f);
        protected override void OnScrolledInput(bool down)
        {
            Vec3 worldPoint = Camera.ScreenToWorld(Viewport.AbsoluteToRelative(CursorPosition), 0.0f);
            _rootTransform.Zoom(down ? 0.1f : -0.1f, worldPoint.Xy, _minScale, _maxScale);
        }

        private ResultFunc _endFunc;
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

        protected override UIMaterialRectangleComponent OnConstruct()
        {
            UIMaterialRectangleComponent root = new UIMaterialRectangleComponent(GetGraphMaterial())
            {
                DockStyle = HudDockStyle.Fill,
                SideAnchorFlags = AnchorFlags.Right | AnchorFlags.Left | AnchorFlags.Top | AnchorFlags.Bottom
            };
            _rootTransform = new UIComponent();
            root.ChildComponents.Add(_rootTransform);
            root.ChildComponents.Add(_bezierRect);
            return root;
        }

        private TMaterial GetGraphMaterial()
        {
            return TMaterial.CreateUnlitColorMaterialForward(Color.Gray);
            return new TMaterial("MatGraphBG");
            string frag = @"
#version 450

uniform vec3 lineColor;
uniform vec3 bgColor;
uniform float scale;
uniform float lineWidth;
uniform vec2 translation;

void main()
{
    vec2 scaledUV = (vPosition.xy + translation) / scale;
    vec2 fractUV = vec2(fract(scaledUV));
    fractUV = (fractUV - 0.5) * 2.0;
    vec2 lines = vec2(floor(abs(fractUV) + lineWidth));
    float lerp = clamp(lines.x + lines.y, 0.0, 1.0);
    vec3 col = mix(bgColor, lineColor, lerp);

    gl_FragColor = vec4(col, 1.0);
}";
        }

        private TMaterial _targetMaterial;
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

        private ResultFunc Decompile(TMaterial mat)
        {
            return new ResultPBRFunc() { Material = _targetMaterial };
        }
    }
}
