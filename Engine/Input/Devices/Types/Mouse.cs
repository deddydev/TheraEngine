using System;
using TheraEngine.Networking;

namespace TheraEngine.Input.Devices
{
    [Serializable]
    public abstract class BaseMouse : InputDevice
    {
        public BaseMouse(int index) : base(index) { }

        /// <summary>
        /// Determines if the mouse cursor will jump to the other side of the Cursor.Clip rectangle.
        /// </summary>
        public bool WrapCursorWithinClip
        {
            get => _cursor.WrapCursorWithinClip;
            set => _cursor.WrapCursorWithinClip = value;
        }

        protected CursorManager _cursor = new CursorManager();
        protected ScrollWheelManager _wheel = new ScrollWheelManager();

        public abstract void SetCursorPosition(float x, float y);

        protected override int GetAxisCount() => 0; 
        protected override int GetButtonCount() => 3;
        public override EDeviceType DeviceType => EDeviceType.Mouse;

        private ButtonManager FindOrCacheButton(EMouseButton button)
        {
            int index = (int)button;
            if (_buttonStates[index] is null)
                _buttonStates[index] = new ButtonManager(index, button.ToString(), SendButtonPressedState, SendButtonAction);
            return _buttonStates[index];
        }
        public void RegisterButtonPressed(EMouseButton button, EInputPauseType pauseType, DelButtonState func, bool unregister)
        {
            if (unregister)
                _buttonStates[(int)button]?.RegisterPressedState(func, pauseType, true);
            else
                FindOrCacheButton(button)?.RegisterPressedState(func, pauseType, false);
        }
        public void RegisterButtonEvent(EMouseButton button, EButtonInputType type, EInputPauseType pauseType, Action func, bool unregister)
            => RegisterButtonEvent(unregister ? _buttonStates[(int)button] : FindOrCacheButton(button), type, pauseType, func, unregister);
        public void RegisterScroll(DelMouseScroll func, EInputPauseType pauseType, bool unregister)
            => _wheel.Register(func, pauseType, unregister);
        public void RegisterMouseMove(DelCursorUpdate func, EInputPauseType pauseType, EMouseMoveType type, bool unregister)
            => _cursor.Register(func, pauseType, type, unregister);

        public ButtonManager LeftClick => _buttonStates[(int)EMouseButton.LeftClick];
        public ButtonManager RightClick => _buttonStates[(int)EMouseButton.RightClick];
        public ButtonManager MiddleClick => _buttonStates[(int)EMouseButton.MiddleClick];

        public bool GetButtonState(EMouseButton button, EButtonInputType type)
            => FindOrCacheButton(button).GetState(type);
    }
    public enum EMouseButton
    {
        LeftClick   = 0,
        RightClick  = 1,
        MiddleClick = 2,
    }
}
