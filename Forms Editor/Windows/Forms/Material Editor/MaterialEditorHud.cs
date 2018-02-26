using System;
using TheraEngine;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models.Materials.Functions;
using TheraEngine.Rendering.UI;

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
        private Vec2 _lastFocusPoint = Vec2.Zero;
        MaterialFunction _selectedFunc = null;
        MaterialFunction _highlightedFunc = null;
        bool _rightClickDown = false;

        private void LeftClickDown()
        {
            _selectedFunc = _highlightedFunc;
            _lastWorldPos = Viewport.ScreenToWorld(Viewport.AbsoluteToRelative(CursorPosition), 0.0f).Xy;
        }
        private UIComponent FindComponent()
        {
            return RootComponent.FindComponent(Viewport.ScreenToWorld(Viewport.AbsoluteToRelative(CursorPosition)).Xy);
        }
        protected override void MouseMove(float x, float y)
        {
            Vec2 pos = Viewport.AbsoluteToRelative(CursorPosition);
            
            if (_selectedFunc != null)
            {
                Vec2 worldPoint = Viewport.ScreenToWorld(pos, 0.0f).Xy;
                _selectedFunc.LocalTranslation += worldPoint - _lastWorldPos;
                _lastWorldPos = worldPoint;
            }
            else if (_rightClickDown)
            {
                Vec2 screenPoint = Viewport.WorldToScreen(_lastFocusPoint).Xy;
                screenPoint += pos - _cursorPos;
                Vec2 newFocusPoint = Viewport.ScreenToWorld(screenPoint).Xy;
                Camera.LocalPoint += _lastFocusPoint - newFocusPoint;
            }
            else
            {
                _highlightedFunc = FindComponent() as MaterialFunction;
                if (_highlightedFunc != null)
                {
                    //Engine.PrintLine(_highlightedFunc.Name + " " + DateTime.Now.ToString());
                }
            }

            _cursorPos = pos;
        }
        private void LeftClickUp()
        {
            _selectedFunc = null;
        }
        private void RightClickDown()
        {
            _rightClickDown = true;
            _lastWorldPos = Camera.LocalPoint.Xy;
            _lastFocusPoint = Viewport.ScreenToWorld(Viewport.AbsoluteToRelative(CursorPosition)).Xy;
        }
        private void RightClickUp()
        {
            _rightClickDown = false;
        }
        protected override void OnScrolledInput(bool down)
        {
            Camera.Zoom(down ? -0.1f : 0.1f, Viewport.AbsoluteToRelative(CursorPosition));
        }

        private ResultBasicFunc _endFunc;
        private ResultBasicFunc EndFunc
        {
            get => _endFunc;
            set
            {
                if (_endFunc != null)
                    RootComponent.ChildComponents.Remove(_endFunc);
                _endFunc = value;
                if (_endFunc != null)
                    RootComponent.ChildComponents.Add(_endFunc);
            }
        }

        protected override UIMaterialRectangleComponent OnConstruct()
        {
            UIMaterialRectangleComponent root = new UIMaterialRectangleComponent()
            {
                DockStyle = HudDockStyle.Fill,
                SideAnchorFlags = AnchorFlags.Right | AnchorFlags.Left | AnchorFlags.Top | AnchorFlags.Bottom
            };
            return root;
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
                        _targetMaterial.EditorMaterialEnd = new ResultBasicFunc() { Material = _targetMaterial };
                    EndFunc = _targetMaterial.EditorMaterialEnd;
                }
                else
                    EndFunc = null;
            }
        }
    }
}
