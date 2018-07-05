using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheraEngine.Networking;

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

        protected override int GetAxisCount() => 0; 
        protected override int GetButtonCount() => 3;
        public override EDeviceType DeviceType => EDeviceType.Mouse;

        private ButtonManager CacheButton(EMouseButton button)
        {
            int index = (int)button;
            if (_buttonStates[index] == null)
                _buttonStates[index] = new ButtonManager(index, button.ToString(), SendButtonPressedState, SendButtonAction);
            return _buttonStates[index];
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
        public static bool WrapCursorWithinClip { get; set; } = true;

        private float _lastX, _lastY;
        readonly List<DelCursorUpdate>[] _onCursorUpdate = new List<DelCursorUpdate>[9];

        internal void Tick(float xPos, float yPos, float delta)
        {
            OnUnbounded(xPos, yPos);

            Point absPt = Cursor.Position;
            Rectangle bounds = Cursor.Clip;
            float relX = 0.0f, relY = 0.0f;

            if (WrapCursorWithinClip)
            {
                //Wrap the X-coord of the cursor
                if (absPt.X >= bounds.Right - 1)
                {
                    while (absPt.X >= bounds.Right - 1)
                        absPt.X -= bounds.Width;
                    absPt.X += 1;
                    relX = (absPt.X - bounds.Left) + (bounds.Right - 1 - _lastX);
                    Cursor.Position = absPt;
                }
                else if (absPt.X <= bounds.Left)
                {
                    while (absPt.X <= bounds.Left)
                        absPt.X += bounds.Width;
                    absPt.X -= 1;
                    relX = (absPt.X - (bounds.Right - 1)) + (bounds.Left - _lastX);
                    Cursor.Position = absPt;
                }
                else
                {
                    relX = absPt.X - _lastX;
                }

                //Wrap the Y-coord of the cursor
                if (absPt.Y >= bounds.Bottom - 1)
                {
                    while (absPt.Y >= bounds.Bottom - 1)
                        absPt.Y -= bounds.Height;
                    absPt.Y += 1;
                    relY = (absPt.Y - bounds.Top) + (bounds.Bottom - 1 - _lastY);
                    Cursor.Position = absPt;
                }
                else if (absPt.Y <= bounds.Top)
                {
                    while (absPt.Y <= bounds.Top)
                        absPt.Y += bounds.Height;
                    absPt.Y -= 1;
                    relY = (absPt.Y - (bounds.Bottom - 1)) + (bounds.Top - _lastY);
                    Cursor.Position = absPt;
                }
                else
                {
                    relY = _lastY - absPt.Y;
                }
            }
            else
            {
                relX = absPt.X - _lastX;
                relY = _lastY - absPt.Y;
            }

            _lastX = absPt.X;
            _lastY = absPt.Y;
            OnRelative(relX, relY);

            BaseRenderPanel pnl = BaseRenderPanel.HoveredPanel;
            if (pnl != null)
                absPt = pnl.PointToClient(absPt);

            xPos = absPt.X;
            yPos = absPt.Y;
            OnAbsolute(xPos, yPos);
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
