using System;
using System.Collections.Generic;
using TheraEngine.Input.Devices.OpenTK;
using TheraEngine.Input.Devices.DirectX;
using TheraEngine.Networking;

namespace TheraEngine.Input.Devices
{
    public enum GamePadButton : int
    {
        DPadUp,
        DPadDown,
        DPadLeft,
        DPadRight,
        FaceUp,
        FaceDown,
        FaceLeft,
        FaceRight,
        LeftStick,
        RightStick,
        SpecialLeft,
        SpecialRight,
        LeftBumper,
        RightBumper
    }
    public enum GamePadAxis : int
    {
        LeftTrigger,
        RightTrigger,
        LeftThumbstickX,
        LeftThumbstickY,
        RightThumbstickX,
        RightThumbstickY,
    }
    public delegate void ConnectedStateChange(bool nowConnected);
    /// <summary>
    /// Input for local
    /// </summary>
    public abstract class BaseGamePad : InputDevice
    {
        public BaseGamePad(int index) : base(index) { }

        public static BaseGamePad NewInstance(int index)
        {
            switch (Engine.InputLibrary)
            {
                case InputLibrary.OpenTK: return new TKGamepad(index);
                case InputLibrary.XInput: return new DXGamepad(index);
            }
            return null;
        }

        protected override int GetButtonCount() => 14;
        protected override int GetAxisCount() => 6;
        public override EDeviceType DeviceType => EDeviceType.Gamepad;

        protected abstract bool ButtonExists(GamePadButton button);
        protected abstract List<bool> ButtonsExist(List<GamePadButton> buttons);
        protected abstract bool AxistExists(GamePadAxis axis);
        protected abstract List<bool> AxesExist(List<GamePadAxis> axes);

        private ButtonManager CacheButton(GamePadButton button)
        {
            int index = (int)button;
            if (_buttonStates[index] == null && ButtonExists(button))
               _buttonStates[index] = new ButtonManager(index, button.ToString(), SendButtonPressedState, SendButtonAction);
            return _buttonStates[index];
        }

        private AxisManager CacheAxis(GamePadAxis axis)
        {
            int index = (int)axis;
            if (_axisStates[index] == null && AxistExists(axis))
                _axisStates[index] = new AxisManager(index, axis.ToString(), SendAxisButtonPressedState, SendAxisButtonAction, SendAxisValue);
            return _axisStates[index];
        }

        public void RegisterButtonEvent(GamePadButton button, ButtonInputType type, EInputPauseType pauseType, Action func, bool unregister)
        {
            RegisterButtonEvent(unregister ? _buttonStates[(int)button] : CacheButton(button), type, pauseType, func, unregister);
        }
        public void RegisterButtonEvent(GamePadAxis axis, ButtonInputType type, EInputPauseType pauseType, Action func, bool unregister)
        {
            RegisterButtonEvent(unregister ? _axisStates[(int)axis] : CacheAxis(axis), type, pauseType, func, unregister);
        }
        public void RegisterButtonState(GamePadButton button, EInputPauseType pauseType, DelButtonState func, bool unregister)
        {
            if (unregister)
                _buttonStates[(int)button]?.RegisterPressedState(func, pauseType, true);
            else
                CacheButton(button)?.RegisterPressedState(func, pauseType, false);
        }
        public void RegisterButtonState(GamePadAxis axis, EInputPauseType pauseType, DelButtonState func, bool unregister)
        {
            if (unregister)
                _axisStates[(int)axis]?.RegisterPressedState(func, pauseType, true);
            else
                CacheAxis(axis)?.RegisterPressedState(func, pauseType, false);
        }
        public void RegisterAxisUpdate(GamePadAxis axis, EInputPauseType pauseType, DelAxisValue func, bool continuousUpdate, bool unregister)
        {
            if (unregister)
                _axisStates[(int)axis]?.RegisterAxis(func, pauseType, continuousUpdate, true);
            else
                CacheAxis(axis)?.RegisterAxis(func, pauseType, continuousUpdate, false);
        }

        /// <summary>
        /// Left motor is low freq, right motor is high freq.
        /// They are NOT the same.
        /// </summary>
        /// <param name="left">Low frequency motor speed, 0 - 1.</param>
        /// <param name="right">High frequency motor speed, 0 - 1.</param>
        public abstract void Vibrate(float lowFreq, float highFreq);
        public void ClearVibration() { Vibrate(0.0f, 0.0f); }

        public ButtonManager DPadUp => _buttonStates[(int)GamePadButton.DPadUp];
        public ButtonManager DPadDown => _buttonStates[(int)GamePadButton.DPadDown];
        public ButtonManager DPadLeft => _buttonStates[(int)GamePadButton.DPadLeft];
        public ButtonManager DPadRight => _buttonStates[(int)GamePadButton.DPadRight];

        public ButtonManager FaceUp => _buttonStates[(int)GamePadButton.FaceUp];
        public ButtonManager FaceDown => _buttonStates[(int)GamePadButton.FaceDown];
        public ButtonManager FaceLeft => _buttonStates[(int)GamePadButton.FaceLeft];
        public ButtonManager FaceRight => _buttonStates[(int)GamePadButton.FaceRight];

        public ButtonManager LeftStick => _buttonStates[(int)GamePadButton.LeftStick];
        public ButtonManager RightStick => _buttonStates[(int)GamePadButton.RightStick];
        public ButtonManager LeftBumper => _buttonStates[(int)GamePadButton.LeftBumper];
        public ButtonManager RightBumper => _buttonStates[(int)GamePadButton.RightBumper];

        public ButtonManager SpecialLeft => _buttonStates[(int)GamePadButton.SpecialLeft];
        public ButtonManager SpecialRight => _buttonStates[(int)GamePadButton.SpecialRight];

        public AxisManager LeftTrigger => _axisStates[(int)GamePadAxis.LeftTrigger];
        public AxisManager RightTrigger => _axisStates[(int)GamePadAxis.RightTrigger];
        public AxisManager LeftThumbstickY => _axisStates[(int)GamePadAxis.LeftThumbstickY];
        public AxisManager LeftThumbstickX => _axisStates[(int)GamePadAxis.LeftThumbstickX];
        public AxisManager RightThumbstickY => _axisStates[(int)GamePadAxis.RightThumbstickY];
        public AxisManager RightThumbstickX => _axisStates[(int)GamePadAxis.RightThumbstickX];
    }
}
