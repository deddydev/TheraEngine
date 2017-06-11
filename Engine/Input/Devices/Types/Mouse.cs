using TheraEngine.Input.Devices.OpenTK;
using TheraEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheraEngine.Input.Devices
{
    public abstract class CMouse : InputDevice
    {
        public CMouse(int index) : base(index) { }

        protected CursorManager _cursor = new CursorManager();
        protected ScrollWheelManager _wheel = new ScrollWheelManager();

        public static Point _origin = Control.MousePosition;
        public static Rectangle _clipRegion = Cursor.Clip;

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
        public void RegisterButtonPressed(EMouseButton button, InputPauseType pauseType, DelButtonState func, bool unregister)
        {
            if (unregister)
                _buttonStates[(int)button]?.RegisterPressedState(func, pauseType, true);
            else
                CacheButton(button)?.RegisterPressedState(func, pauseType, false);
        }
        public void RegisterButtonEvent(EMouseButton button, ButtonInputType type, InputPauseType pauseType, Action func, bool unregister)
        {
            RegisterButtonEvent(unregister ? _buttonStates[(int)button] : CacheButton(button), type, pauseType, func, unregister);
        }
        public void RegisterScroll(DelMouseScroll func, InputPauseType pauseType, bool unregister)
        {
            _wheel.Register(func, pauseType, unregister);
        }
        public void RegisterMouseMove(DelCursorUpdate func, InputPauseType pauseType, bool relative, bool unregister)
        {
            _cursor.Register(func, pauseType, relative, unregister);
        }
        public ButtonManager LeftClick => _buttonStates[(int)EMouseButton.LeftClick];
        public ButtonManager RightClick => _buttonStates[(int)EMouseButton.RightClick];
        public ButtonManager MiddleClick => _buttonStates[(int)EMouseButton.MiddleClick];
    }
    public delegate void DelMouseScroll(bool up);
    public delegate void DelCursorUpdate(float x, float y);
    public class CursorManager
    {
        private float _lastX, _lastY;

        List<DelCursorUpdate>[] _onCursorUpdate = new List<DelCursorUpdate>[6];

        internal void Tick(float xPos, float yPos, float delta)
        {
            Point absolute = Cursor.Position;
            if (RenderPanel.HoveredPanel != null)
                absolute = (Point)RenderPanel.HoveredPanel.Invoke(RenderPanel.HoveredPanel.PointToClientDelegate, absolute);
            OnAbsolute(absolute.X, absolute.Y);
            OnRelative(xPos - _lastX, yPos - _lastY);
            _lastX = xPos;
            _lastY = yPos;
        }
        public void Register(DelCursorUpdate func, InputPauseType pauseType, bool relative, bool unregister)
        {
            int index = (relative ? 0 : 3) + (int)pauseType;
            if (unregister)
            {
                List<DelCursorUpdate> list = _onCursorUpdate[index];
                if (list == null)
                    return;
                list.Remove(func);
                if (list.Count == 0)
                    _onCursorUpdate[index] = null;
            }
            else
            {
                if (_onCursorUpdate[index] == null)
                    _onCursorUpdate[index] = new List<DelCursorUpdate>() { func };
                else
                    _onCursorUpdate[index].Add(func);
            }
        }
        private void OnAbsolute(float x, float y)
        {
            PerformAction(false, x, y);
        }
        private void OnRelative(float x, float y)
        {
            PerformAction(true, x, y);
        }
        protected void PerformAction(bool relative, float x, float y)
        {
            int index = relative ? 0 : 3;
            List<DelCursorUpdate> list = _onCursorUpdate[index];
            if (list != null)
            {
                int i = list.Count;
                for (int j = 0; j < i; ++j)
                    list[j](x, y);
            }
            list = Engine.IsPaused ?
                _onCursorUpdate[index + (int)InputPauseType.TickOnlyWhenPaused] :
                _onCursorUpdate[index + (int)InputPauseType.TickOnlyWhenUnpaused];
            if (list != null)
            {
                int i = list.Count;
                for (int j = 0; j < i; ++j)
                    list[j](x, y);
            }
            //Console.WriteLine(_name + ": " + type.ToString());
        }
    }
    public class ScrollWheelManager
    {
        List<Tuple<InputPauseType, DelMouseScroll>> _onUpdate = new List<Tuple<InputPauseType, DelMouseScroll>>();

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
        public void Register(DelMouseScroll func, InputPauseType pauseType, bool unregister)
        {
            if (unregister)
            {
                int index = _onUpdate.FindIndex(x => x.Item2 == func);
                if (index >= 0 && index < _onUpdate.Count)
                    _onUpdate.RemoveAt(index);
            }
            else
                _onUpdate.Add(new Tuple<InputPauseType, DelMouseScroll>(pauseType, func));
        }
        private void OnUpdate(bool up)
        {
            int i = _onUpdate.Count;
            for (int x = 0; x < i; ++x)
            {
                var update = _onUpdate[x];
                if (update.Item1 == InputPauseType.TickAlways ||
                    (update.Item1 == InputPauseType.TickOnlyWhenUnpaused && !Engine.IsPaused) ||
                    (update.Item1 == InputPauseType.TickOnlyWhenPaused && Engine.IsPaused))
                    update.Item2(up);
            }
        }
    }
    public enum EMouseButton
    {
        LeftClick   = 0,
        RightClick  = 1,
        MiddleClick = 2,
    }
}
