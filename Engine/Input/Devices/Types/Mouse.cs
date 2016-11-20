using CustomEngine.Input.Devices.OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Input.Devices
{
    public abstract class CMouse : InputDevice
    {
        public CMouse(int index) : base(index) { }

        protected CursorManager _cursor = new CursorManager();
        protected ScrollWheelManager _wheel = new ScrollWheelManager();

        public abstract void SetCursorPosition(float x, float y);

        protected override int GetAxisCount() { return 0; }
        protected override int GetButtonCount() { return 3; }

        private ButtonManager CacheButton(EMouseButton button)
        {
            int b = (int)button;
            if (_buttonStates[b] == null)
                _buttonStates[b] = new ButtonManager(button.ToString());
            return _buttonStates[b];
        }
        public void RegisterButtonPressed(EMouseButton button, DelButtonState func)
        {
            CacheButton(button)?.RegisterPressedState(func);
        }
        public void RegisterButtonEvent(EMouseButton button, ButtonInputType type, Action func)
        {
            RegisterButtonEvent(CacheButton(button), type, func);
        }
        public void RegisterScroll(DelMouseScroll func)
        {
            _wheel.Register(func);
        }
        public void RegisterMouseMove(DelCursorUpdate func, bool relative)
        {
            _cursor.Register(func, relative);
        }
        public ButtonManager LeftClick { get { return _buttonStates[(int)EMouseButton.LeftClick]; } }
        public ButtonManager RightClick { get { return _buttonStates[(int)EMouseButton.RightClick]; } }
        public ButtonManager MiddleClick { get { return _buttonStates[(int)EMouseButton.MiddleClick]; } }
    }
    public delegate void DelMouseScroll(bool up);
    public delegate void DelCursorUpdate(float x, float y);
    public class CursorManager
    {
        private float _x, _y;

        List<DelCursorUpdate>
            _onAbsolute = new List<DelCursorUpdate>(),
            _onRelative = new List<DelCursorUpdate>();

        internal void Tick(float x, float y, float delta)
        {
            float xDiff = x - _x;
            float yDiff = y - _y;
            _x = x;
            _y = y;
            OnAbsolute(x, y);
            OnRelative(xDiff, yDiff);
        }
        public void Register(DelCursorUpdate func, bool relative)
        {
            if (relative)
                _onRelative.Add(func);
            else
                _onAbsolute.Add(func);
        }
        private void OnAbsolute(float x, float y)
        {
            _onAbsolute.ForEach(del => del(x, y));
        }
        private void OnRelative(float x, float y)
        {
            _onRelative.ForEach(del => del(x, y));
        }
    }
    public class ScrollWheelManager
    {
        List<DelMouseScroll> _onUpdate = new List<DelMouseScroll>();

        float _lastValue = 0.0f;

        internal void Tick(float value, float delta)
        {
            if (value.EqualTo(_lastValue))
                return;
            if (value < _lastValue)
            {
                OnUpdate(true);
                _lastValue = value;
            }
            else if (value > _lastValue)
            {
                OnUpdate(false);
                _lastValue = value;
            }
        }
        public void Register(DelMouseScroll func)
        {
            _onUpdate.Add(func);
        }
        private void OnUpdate(bool up)
        {
            _onUpdate.ForEach(del => del(up));
        }
    }
    public enum EMouseButton
    {
        LeftClick   = 0,
        RightClick  = 1,
        MiddleClick = 2,
    }
}
