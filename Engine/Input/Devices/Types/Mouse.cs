using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TheraEngine.Input.Devices
{
    public abstract class BaseMouse : InputDevice
    {
        public BaseMouse(int index) : base(index) { }

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
        public void RegisterButtonPressed(EMouseButton button, EInputPauseType pauseType, DelButtonState func, bool unregister)
        {
            if (unregister)
                _buttonStates[(int)button]?.RegisterPressedState(func, pauseType, true);
            else
                CacheButton(button)?.RegisterPressedState(func, pauseType, false);
        }
        public void RegisterButtonEvent(EMouseButton button, ButtonInputType type, EInputPauseType pauseType, Action func, bool unregister)
        {
            RegisterButtonEvent(unregister ? _buttonStates[(int)button] : CacheButton(button), type, pauseType, func, unregister);
        }
        public void RegisterScroll(DelMouseScroll func, EInputPauseType pauseType, bool unregister)
        {
            _wheel.Register(func, pauseType, unregister);
        }
        public void RegisterMouseMove(DelCursorUpdate func, EInputPauseType pauseType, MouseMoveType type, bool unregister)
        {
            _cursor.Register(func, pauseType, type, unregister);
        }
        public ButtonManager LeftClick => _buttonStates[(int)EMouseButton.LeftClick];
        public ButtonManager RightClick => _buttonStates[(int)EMouseButton.RightClick];
        public ButtonManager MiddleClick => _buttonStates[(int)EMouseButton.MiddleClick];
    }
    public delegate void DelMouseScroll(bool down);
    public delegate void DelCursorUpdate(float x, float y);
    public class CursorManager
    {
        private float _lastX, _lastY;

        List<DelCursorUpdate>[] _onCursorUpdate = new List<DelCursorUpdate>[9];

        internal void Tick(float xPos, float yPos, float delta)
        {
            OnUnbounded(xPos, yPos);
            Point absolute = Cursor.Position;
            if (BaseRenderPanel.HoveredPanel != null)
            //    absolute = (Point)BaseRenderPanel.HoveredPanel.Invoke(BaseRenderPanel.HoveredPanel.PointToClientDelegate, absolute);
            absolute = BaseRenderPanel.HoveredPanel.PointToClient(absolute);
            xPos = absolute.X;
            yPos = absolute.Y;
            OnAbsolute(xPos, yPos);
            //TODO: make relative unbounded
            OnRelative(xPos - _lastX, yPos - _lastY);
            _lastX = xPos;
            _lastY = yPos;
        }
        public void Register(DelCursorUpdate func, EInputPauseType pauseType, MouseMoveType type, bool unregister)
        {
            int index = ((int)type * 3) + (int)pauseType;
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
            PerformAction(MouseMoveType.Absolute, x, y);
        }
        private void OnRelative(float x, float y)
        {
            PerformAction(MouseMoveType.Relative, x, y);
        }
        private void OnUnbounded(float x, float y)
        {
            PerformAction(MouseMoveType.Unbounded, x, y);
        }
        protected void PerformAction(MouseMoveType type, float x, float y)
        {
            int index = (int)type * 3;
            List<DelCursorUpdate> list = _onCursorUpdate[index];
            if (list != null)
            {
                int i = list.Count;
                for (int j = 0; j < i; ++j)
                    list[j](x, y);
            }
            index += (int)(Engine.IsPaused ? EInputPauseType.TickOnlyWhenPaused : EInputPauseType.TickOnlyWhenUnpaused);
            list = _onCursorUpdate[index];
            if (list != null)
            {
                int i = list.Count;
                for (int j = 0; j < i; ++j)
                    list[j](x, y);
            }
            //Engine.DebugPrint(_name + ": " + type.ToString());
        }
    }
    public class ScrollWheelManager
    {
        List<Tuple<EInputPauseType, DelMouseScroll>> _onUpdate = new List<Tuple<EInputPauseType, DelMouseScroll>>();

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
        public void Register(DelMouseScroll func, EInputPauseType pauseType, bool unregister)
        {
            if (unregister)
            {
                int index = _onUpdate.FindIndex(x => x.Item2 == func);
                if (index >= 0 && index < _onUpdate.Count)
                    _onUpdate.RemoveAt(index);
            }
            else
                _onUpdate.Add(new Tuple<EInputPauseType, DelMouseScroll>(pauseType, func));
        }
        private void OnUpdate(bool down)
        {
            int i = _onUpdate.Count;
            for (int x = 0; x < i; ++x)
            {
                var update = _onUpdate[x];
                if (update.Item1 == EInputPauseType.TickAlways ||
                    (update.Item1 == EInputPauseType.TickOnlyWhenUnpaused && !Engine.IsPaused) ||
                    (update.Item1 == EInputPauseType.TickOnlyWhenPaused && Engine.IsPaused))
                    update.Item2(down);
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
