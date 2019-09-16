using Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheraEngine.Networking;
using TheraEngine.Rendering;

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

        public static Point _origin = Control.MousePosition;
        public static Rectangle _clipRegion = Cursor.Clip;

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
    public delegate void DelMouseScroll(bool down);
    public delegate void DelCursorUpdate(float x, float y);
    [Serializable]
    public class CursorManager
    {
        /// <summary>
        /// Determines if the mouse cursor will jump to the other side of the Cursor.Clip rectangle.
        /// Affects all cursors.
        /// </summary>
        public static bool GlobalWrapCursorWithinClip { get; set; } = false;
        /// <summary>
        /// Determines if the mouse cursor will jump to the other side of the Cursor.Clip rectangle.
        /// Affects only this cursor.
        /// </summary>
        public bool WrapCursorWithinClip { get; set; } = false;

        private float _lastX, _lastY;
        readonly List<DelCursorUpdate>[] _onCursorUpdate = new List<DelCursorUpdate>[9];

        internal void Tick(float xPos, float yPos, float delta)
        {
            OnUnbounded(xPos, yPos);

            Point absPt = Cursor.Position;
            Rectangle bounds = Cursor.Clip;
            float relX = 0.0f, relY = 0.0f;

            if (GlobalWrapCursorWithinClip || WrapCursorWithinClip)
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

            RenderContext pnl = RenderContext.Hovered;
            if (pnl != null)
                absPt = pnl.PointToClient(absPt);

            xPos = absPt.X;
            yPos = absPt.Y;
            OnAbsolute(xPos, yPos);
        }
        
        public void Register(DelCursorUpdate func, EInputPauseType pauseType, EMouseMoveType type, bool unregister)
        {
            int index = ((int)type * 3) + (int)pauseType;
            if (unregister)
            {
                List<DelCursorUpdate> list = _onCursorUpdate[index];
                if (list is null)
                    return;
                list.Remove(func);
                if (list.Count == 0)
                    _onCursorUpdate[index] = null;
            }
            else
            {
                if (_onCursorUpdate[index] is null)
                    _onCursorUpdate[index] = new List<DelCursorUpdate>() { func };
                else
                    _onCursorUpdate[index].Add(func);
            }
        }
        private void OnAbsolute(float x, float y)
            => PerformAction(EMouseMoveType.Absolute, x, y);
        private void OnRelative(float x, float y)
            => PerformAction(EMouseMoveType.Relative, x, y);
        private void OnUnbounded(float x, float y)
            => PerformAction(EMouseMoveType.Unbounded, x, y);
        protected void PerformAction(EMouseMoveType type, float x, float y)
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
    [Serializable]
    public class ScrollWheelManager
    {
        List<(EInputPauseType PauseType, DelMouseScroll Method)> _onUpdate = new List<(EInputPauseType, DelMouseScroll)>();

        float _lastValue = 0.0f;

        public static float ScrollSpeed { get; set; } = 60.0f;

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
                int index = _onUpdate.FindIndex(x => x.Method == func);
                if (index >= 0 && index < _onUpdate.Count)
                    _onUpdate.RemoveAt(index);
            }
            else
                _onUpdate.Add((pauseType, func));
        }
        private void OnUpdate(bool down)
        {
            int i = _onUpdate.Count;
            for (int x = 0; x < i; ++x)
            {
                var (PauseType, Method) = _onUpdate[x];
                if (PauseType == EInputPauseType.TickAlways ||
                    (PauseType == EInputPauseType.TickOnlyWhenUnpaused && !Engine.IsPaused) ||
                    (PauseType == EInputPauseType.TickOnlyWhenPaused && Engine.IsPaused))
                    Method(down);
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
