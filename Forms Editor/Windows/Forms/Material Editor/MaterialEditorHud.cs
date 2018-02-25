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
            input.RegisterButtonEvent(EMouseButton.LeftClick, ButtonInputType.Pressed, MouseDown, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EMouseButton.LeftClick, ButtonInputType.Released, MouseUp, EInputPauseType.TickAlways);
            input.RegisterMouseScroll(OnScrolledInput, EInputPauseType.TickAlways);
            input.RegisterMouseMove(MouseMove, false, EInputPauseType.TickAlways);
        }

        MaterialFunction _selectedFunc = null;
        private void MouseDown()
        {
            UIComponent comp = RootComponent.FindComponent(LocalPlayerController.Viewport.AbsoluteToRelative(CursorPosition));
            if (comp != null && comp is MaterialFunction func)
            {
                _selectedFunc = func;
            }
            else
                _selectedFunc = null;
        }
        protected override void MouseMove(float x, float y)
        {
            //UIComponent comp = RootComponent.FindComponent(LocalPlayerController.Viewport.AbsoluteToRelative(CursorPosition));
            //if (comp != null && comp is MaterialFunction func)
            //{
            //    Engine.PrintLine(func.Name);
            //}

            Vec2 pos = LocalPlayerController.Viewport.AbsoluteToRelative(CursorPosition);
            
            if (_selectedFunc != null)
            {
                //Vec3 worldPoint = LocalPlayerController.Viewport.ScreenToWorld(pos, 0.0f);
                _selectedFunc.LocalTranslation += pos - _cursorPos; //worldPoint.Xy;
            }

            _cursorPos = pos;
            //base.MouseMove(x, y);
        }
        private void MouseUp()
        {
            _selectedFunc = null;
        }

        private ResultBasicFunc _endFunc;
        private ResultBasicFunc EndFunc
        {
            get => _endFunc;
            set
            {
                if (_endFunc != null)
                {
                    RootComponent.ChildComponents.Remove(_endFunc);
                }
                _endFunc = value;
                if (_endFunc != null)
                {
                    RootComponent.ChildComponents.Add(_endFunc);
                }
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
