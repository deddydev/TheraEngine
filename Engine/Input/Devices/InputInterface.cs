using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomEngine.Input.Devices
{
    public enum ButtonInputType
    {
        Pressed         = 0,
        Released        = 1,
        Held            = 2,
        DoublePressed   = 3,
    }
    public enum InputDeviceType
    {
        Gamepad,
        Keyboard,
        Mouse,
    }
    public delegate void DelWantsInputsRegistered(InputInterface input);
    public class LabeledInput
    {
        string _label;
    }
    public enum MouseMoveType
    {
        Relative,
        Absolute,
    }
    /// <summary>
    /// Input for server
    /// </summary>
    public class InputInterface : ObjectBase
    {
        public int PlayerIndex
        {
            get { return _playerIndex; }
            set
            {
                _playerIndex = value;
                UpdateDevices();
            }
        }

        Dictionary<string, List<EKey>> _namedKeys = new Dictionary<string, List<EKey>>();
        Dictionary<string, List<GamePadButton>> _namedGamepadButtons = new Dictionary<string, List<GamePadButton>>();
        Dictionary<string, List<GamePadAxis>> _namedGamepadAxes = new Dictionary<string, List<GamePadAxis>>();
        Dictionary<string, List<EMouseButton>> _namedMouseButtons = new Dictionary<string, List<EMouseButton>>();

        public event DelWantsInputsRegistered WantsInputsRegistered;

        private bool _unregister = false;
        private int _playerIndex;
        private CGamePad _gamepad;
        private CKeyboard _keyboard;
        private CMouse _mouse;

        public InputInterface(int playerIndex) { PlayerIndex = playerIndex; }

        public void UpdateDevices()
        {
            TryUnregisterInput();
            GetDevices();
            TryRegisterInput();
        }
        public void TryRegisterInput()
        {
            if (_gamepad != null || _keyboard != null || _mouse != null)
            {
                _unregister = false;
                WantsInputsRegistered?.Invoke(this);
            }
        }
        public void TryUnregisterInput()
        {
            if (_gamepad != null || _keyboard != null || _mouse != null)
            {
                _unregister = true;
                WantsInputsRegistered?.Invoke(this);
                _unregister = false;
            }
        }
        private void GetDevices()
        {
            InputDevice[] gamepads = InputDevice.CurrentDevices[InputDeviceType.Gamepad];
            InputDevice[] keyboards = InputDevice.CurrentDevices[InputDeviceType.Keyboard];
            InputDevice[] mice = InputDevice.CurrentDevices[InputDeviceType.Mouse];

            if (_playerIndex >= 0 && _playerIndex < gamepads.Length)
                _gamepad = gamepads[_playerIndex] as CGamePad;
            if (_playerIndex >= 0 && _playerIndex < keyboards.Length)
                _keyboard = keyboards[_playerIndex] as CKeyboard;
            if (_playerIndex >= 0 && _playerIndex < mice.Length)
                _mouse = mice[_playerIndex] as CMouse;
        }

        #region Mouse input registration
        public void RegisterButtonPressed(EMouseButton button, DelButtonState func, InputPauseType pauseType = InputPauseType.TickOnlyWhenUnpaused)
        {
            _mouse?.RegisterButtonPressed(button, pauseType, func, _unregister);
        }
        public void RegisterButtonEvent(EMouseButton button, ButtonInputType type, Action func, InputPauseType pauseType = InputPauseType.TickOnlyWhenUnpaused)
        {
            _mouse?.RegisterButtonEvent(button, type, pauseType, func, _unregister);
        }
        public void RegisterMouseScroll(DelMouseScroll func, InputPauseType pauseType = InputPauseType.TickOnlyWhenUnpaused)
        {
            _mouse?.RegisterScroll(func, pauseType, _unregister);
        }
        public void RegisterMouseMove(DelCursorUpdate func, bool relative, InputPauseType pauseType = InputPauseType.TickOnlyWhenUnpaused)
        {
            _mouse?.RegisterMouseMove(func, pauseType, relative, _unregister);
        }
        #endregion

        #region Keyboard input registration
        public void RegisterButtonPressed(EKey button, DelButtonState func, InputPauseType pauseType = InputPauseType.TickOnlyWhenUnpaused)
        {
            _keyboard?.RegisterButtonPressed(button, pauseType, func, _unregister);
        }
        public void RegisterButtonEvent(EKey button, ButtonInputType type, Action func, InputPauseType pauseType = InputPauseType.TickOnlyWhenUnpaused)
        {
            _keyboard?.RegisterButtonEvent(button, type, pauseType, func, _unregister);
        }
        #endregion

        #region Gamepad input registration
        public void RegisterButtonPressed(GamePadAxis axis, DelButtonState func, InputPauseType pauseType = InputPauseType.TickOnlyWhenUnpaused)
        {
            _gamepad?.RegisterButtonState(axis, pauseType, func, _unregister);
        }
        public void RegisterButtonPressed(GamePadButton button, DelButtonState func, InputPauseType pauseType = InputPauseType.TickOnlyWhenUnpaused)
        {
            _gamepad?.RegisterButtonState(button, pauseType, func, _unregister);
        }
        public void RegisterButtonEvent(GamePadButton button, ButtonInputType type, Action func, InputPauseType pauseType = InputPauseType.TickOnlyWhenUnpaused)
        {
            _gamepad?.RegisterButtonEvent(button, type, pauseType, func, _unregister);
        }
        public void RegisterButtonEvent(GamePadAxis button, ButtonInputType type, Action func, InputPauseType pauseType = InputPauseType.TickOnlyWhenUnpaused)
        {
            _gamepad?.RegisterButtonEvent(button, type, pauseType, func, _unregister);
        }
        public void RegisterAxisUpdate(GamePadAxis axis, DelAxisValue func, bool continuousUpdate, InputPauseType pauseType = InputPauseType.TickOnlyWhenUnpaused)
        {
            _gamepad?.RegisterAxisUpdate(axis, pauseType, func, continuousUpdate, _unregister);
        }
        #endregion
    }
}
