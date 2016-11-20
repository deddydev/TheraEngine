using System;
using System.Collections.Generic;
using System.Linq;
using CustomEngine.Input.Devices.OpenTK;
using CustomEngine.Input.Devices.DirectX;

namespace CustomEngine.Input.Devices
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
    public abstract class CGamePad : InputDevice
    {
        public CGamePad(int index) : base(index) { }
        public static CGamePad NewInstance(int index)
        {
            switch (Engine.InputLibrary)
            {
                case InputLibrary.OpenTK: return new TKGamepad(index);
                case InputLibrary.XInput: return new DXGamepad(index);
            }
            return null;
        }
        protected override int GetButtonCount() { return 14; }
        protected override int GetAxisCount() { return 6; }
        protected abstract bool ButtonExists(GamePadButton button);
        protected abstract List<bool> ButtonsExist(List<GamePadButton> buttons);
        protected abstract bool AxistExists(GamePadAxis axis);
        protected abstract List<bool> AxesExist(List<GamePadAxis> axes);
        private ButtonManager CacheButton(GamePadButton button)
        {
            int b = (int)button;
            if (_buttonStates[b] == null && ButtonExists(button))
               _buttonStates[b] = new ButtonManager(button.ToString());
            return _buttonStates[b];
        }
        private AxisManager CacheAxis(GamePadAxis axis)
        {
            int a = (int)axis;
            if (_axisStates[a] == null && AxistExists(axis))
                _axisStates[a] = new AxisManager(axis.ToString());
            return _axisStates[a];
        }
        public void RegisterButtonEvent(GamePadButton button, ButtonInputType type, Action func)
        {
            RegisterButtonEvent(CacheButton(button), type, func);
        }
        public void RegisterButtonEvent(GamePadAxis axis, ButtonInputType type, Action func)
        {
            RegisterButtonEvent(CacheAxis(axis), type, func);
        }
        public void RegisterButtonState(GamePadButton button, DelButtonState func)
        {
            CacheButton(button)?.RegisterPressedState(func);
        }
        public void RegisterButtonState(GamePadAxis axis, DelButtonState func)
        {
            CacheAxis(axis)?.RegisterPressedState(func);
        }
        public void RegisterAxisUpdate(GamePadAxis axis, DelAxisValue func, bool continuousUpdate)
        {
            CacheAxis(axis)?.RegisterAxis(func, continuousUpdate);
        }
        /// <summary>
        /// Left motor is low freq, right motor is high freq.
        /// They are NOT the same.
        /// </summary>
        /// <param name="left">Low frequency motor speed, 0 - 1.</param>
        /// <param name="right">High frequency motor speed, 0 - 1.</param>
        public abstract void Vibrate(float lowFreq, float highFreq);
        public void ClearVibration() { Vibrate(0.0f, 0.0f); }

        public ButtonManager DPadUp { get { return _buttonStates[(int)GamePadButton.DPadUp]; } }
        public ButtonManager DPadDown { get { return _buttonStates[(int)GamePadButton.DPadDown]; } }
        public ButtonManager DPadLeft { get { return _buttonStates[(int)GamePadButton.DPadLeft]; } }
        public ButtonManager DPadRight { get { return _buttonStates[(int)GamePadButton.DPadRight]; } }

        public ButtonManager FaceUp { get { return _buttonStates[(int)GamePadButton.FaceUp]; } }
        public ButtonManager FaceDown { get { return _buttonStates[(int)GamePadButton.FaceDown]; } }
        public ButtonManager FaceLeft { get { return _buttonStates[(int)GamePadButton.FaceLeft]; } }
        public ButtonManager FaceRight { get { return _buttonStates[(int)GamePadButton.FaceRight]; } }

        public ButtonManager LeftStick { get { return _buttonStates[(int)GamePadButton.LeftStick]; } }
        public ButtonManager RightStick { get { return _buttonStates[(int)GamePadButton.RightStick]; } }
        public ButtonManager LeftBumper { get { return _buttonStates[(int)GamePadButton.LeftBumper]; } }
        public ButtonManager RightBumper { get { return _buttonStates[(int)GamePadButton.RightBumper]; } }

        public ButtonManager SpecialLeft { get { return _buttonStates[(int)GamePadButton.SpecialLeft]; } }
        public ButtonManager SpecialRight { get { return _buttonStates[(int)GamePadButton.SpecialRight]; } }

        public AxisManager LeftTrigger { get { return _axisStates[(int)GamePadAxis.LeftTrigger]; } }
        public AxisManager RightTrigger { get { return _axisStates[(int)GamePadAxis.RightTrigger]; } }
        public AxisManager LeftThumbstickY { get { return _axisStates[(int)GamePadAxis.LeftThumbstickY]; } }
        public AxisManager LeftThumbstickX { get { return _axisStates[(int)GamePadAxis.LeftThumbstickX]; } }
        public AxisManager RightThumbstickY { get { return _axisStates[(int)GamePadAxis.RightThumbstickY]; } }
        public AxisManager RightThumbstickX { get { return _axisStates[(int)GamePadAxis.RightThumbstickX]; } }
    }
}
