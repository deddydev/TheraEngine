using System;
using System.Collections.Generic;
using TheraEngine.Input.Devices.OpenTK;
using TheraEngine.Input.Devices.DirectX;
using TheraEngine.Networking;

namespace TheraEngine.Input.Devices
{
    public enum EGamePadButton : int
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
    public enum EGamePadAxis : int
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
                case EInputLibrary.OpenTK: return new TKGamepad(index);
                case EInputLibrary.XInput: return new DXGamepad(index);
            }
            return null;
        }

        protected override int GetButtonCount() => 14;
        protected override int GetAxisCount() => 6;
        public override EDeviceType DeviceType => EDeviceType.Gamepad;

        protected abstract bool ButtonExists(EGamePadButton button);
        protected abstract List<bool> ButtonsExist(List<EGamePadButton> buttons);
        protected abstract bool AxistExists(EGamePadAxis axis);
        protected abstract List<bool> AxesExist(List<EGamePadAxis> axes);

        private ButtonManager FindOrCacheButton(EGamePadButton button)
        {
            int index = (int)button;
            if (_buttonStates[index] == null && ButtonExists(button))
               _buttonStates[index] = new ButtonManager(index, button.ToString(), SendButtonPressedState, SendButtonAction);
            return _buttonStates[index];
        }

        private AxisManager FindOrCacheAxis(EGamePadAxis axis)
        {
            int index = (int)axis;
            if (_axisStates[index] == null && AxistExists(axis))
                _axisStates[index] = new AxisManager(index, axis.ToString(), SendAxisButtonPressedState, SendAxisButtonAction, SendAxisValue);
            return _axisStates[index];
        }

        public void RegisterButtonEvent(EGamePadButton button, EButtonInputType type, EInputPauseType pauseType, Action func, bool unregister)
        {
            RegisterButtonEvent(unregister ? _buttonStates[(int)button] : FindOrCacheButton(button), type, pauseType, func, unregister);
        }
        public void RegisterButtonEvent(EGamePadAxis axis, EButtonInputType type, EInputPauseType pauseType, Action func, bool unregister)
        {
            RegisterButtonEvent(unregister ? _axisStates[(int)axis] : FindOrCacheAxis(axis), type, pauseType, func, unregister);
        }
        public void RegisterButtonState(EGamePadButton button, EInputPauseType pauseType, DelButtonState func, bool unregister)
        {
            if (unregister)
                _buttonStates[(int)button]?.RegisterPressedState(func, pauseType, true);
            else
                FindOrCacheButton(button)?.RegisterPressedState(func, pauseType, false);
        }
        public void RegisterButtonState(EGamePadAxis axis, EInputPauseType pauseType, DelButtonState func, bool unregister)
        {
            if (unregister)
                _axisStates[(int)axis]?.RegisterPressedState(func, pauseType, true);
            else
                FindOrCacheAxis(axis)?.RegisterPressedState(func, pauseType, false);
        }
        public void RegisterAxisUpdate(EGamePadAxis axis, EInputPauseType pauseType, DelAxisValue func, bool continuousUpdate, bool unregister)
        {
            if (unregister)
                _axisStates[(int)axis]?.RegisterAxis(func, pauseType, continuousUpdate, true);
            else
                FindOrCacheAxis(axis)?.RegisterAxis(func, pauseType, continuousUpdate, false);
        }

        /// <summary>
        /// Left motor is low freq, right motor is high freq.
        /// They are NOT the same.
        /// </summary>
        /// <param name="left">Low frequency motor speed, 0 - 1.</param>
        /// <param name="right">High frequency motor speed, 0 - 1.</param>
        public abstract void Vibrate(float lowFreq, float highFreq);
        public void ClearVibration() => Vibrate(0.0f, 0.0f);

        public ButtonManager DPadUp         => _buttonStates[(int)EGamePadButton.DPadUp];
        public ButtonManager DPadDown       => _buttonStates[(int)EGamePadButton.DPadDown];
        public ButtonManager DPadLeft       => _buttonStates[(int)EGamePadButton.DPadLeft];
        public ButtonManager DPadRight      => _buttonStates[(int)EGamePadButton.DPadRight];

        public ButtonManager FaceUp         => _buttonStates[(int)EGamePadButton.FaceUp];
        public ButtonManager FaceDown       => _buttonStates[(int)EGamePadButton.FaceDown];
        public ButtonManager FaceLeft       => _buttonStates[(int)EGamePadButton.FaceLeft];
        public ButtonManager FaceRight      => _buttonStates[(int)EGamePadButton.FaceRight];

        public ButtonManager LeftStick      => _buttonStates[(int)EGamePadButton.LeftStick];
        public ButtonManager RightStick     => _buttonStates[(int)EGamePadButton.RightStick];
        public ButtonManager LeftBumper     => _buttonStates[(int)EGamePadButton.LeftBumper];
        public ButtonManager RightBumper    => _buttonStates[(int)EGamePadButton.RightBumper];

        public ButtonManager SpecialLeft    => _buttonStates[(int)EGamePadButton.SpecialLeft];
        public ButtonManager SpecialRight   => _buttonStates[(int)EGamePadButton.SpecialRight];

        public AxisManager LeftTrigger      => _axisStates[(int)EGamePadAxis.LeftTrigger];
        public AxisManager RightTrigger     => _axisStates[(int)EGamePadAxis.RightTrigger];
        public AxisManager LeftThumbstickY  => _axisStates[(int)EGamePadAxis.LeftThumbstickY];
        public AxisManager LeftThumbstickX  => _axisStates[(int)EGamePadAxis.LeftThumbstickX];
        public AxisManager RightThumbstickY => _axisStates[(int)EGamePadAxis.RightThumbstickY];
        public AxisManager RightThumbstickX => _axisStates[(int)EGamePadAxis.RightThumbstickX];

        public bool GetButtonState(EGamePadButton button, EButtonInputType type)
            => FindOrCacheButton(button).GetState(type);
        public bool GetAxisState(EGamePadAxis axis, EButtonInputType type)
            => FindOrCacheAxis(axis).GetState(type);
        public float GetAxisValue(EGamePadAxis axis)
            => FindOrCacheAxis(axis).Value;
    }
}
