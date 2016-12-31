using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomEngine.Input.Devices
{
    [Flags]
    public enum ButtonInputType
    {
        Pressed         = 1,
        Released        = 2,
        Held            = 4,
        DoublePressed   = 8,
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
            UnregisterAll();
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
            _gamepad = InputDevice.CurrentDevices[InputDeviceType.Gamepad][_playerIndex] as CGamePad;
            _keyboard = InputDevice.CurrentDevices[InputDeviceType.Keyboard][_playerIndex] as CKeyboard;
            _mouse = InputDevice.CurrentDevices[InputDeviceType.Mouse][_playerIndex] as CMouse;
        }

        #region Mouse input registration
        public void RegisterButtonPressed(EMouseButton button, DelButtonState func)
        {
            _mouse?.RegisterButtonPressed(button, func, _unregister);
        }
        public void RegisterButtonEvent(EMouseButton button, ButtonInputType type, Action func)
        {
            _mouse?.RegisterButtonEvent(button, type, func, _unregister);
        }
        public void RegisterMouseScroll(DelMouseScroll func)
        {
            _mouse?.RegisterScroll(func, _unregister);
        }
        public void RegisterMouseMove(DelCursorUpdate func, bool relative)
        {
            _mouse?.RegisterMouseMove(func, relative, _unregister);
        }
        #endregion

        #region Keyboard input registration
        public void RegisterButtonPressed(EKey button, DelButtonState func)
        {
            _keyboard?.RegisterButtonPressed(button, func, _unregister);
        }
        public void RegisterButtonEvent(EKey button, ButtonInputType type, Action func)
        {
            _keyboard?.RegisterButtonEvent(button, type, func, _unregister);
        }
        #endregion

        #region Gamepad input registration
        public void RegisterButtonPressed(GamePadAxis axis, DelButtonState func)
        {
            _gamepad?.RegisterButtonState(axis, func, _unregister);
        }
        public void RegisterButtonPressed(GamePadButton button, DelButtonState func)
        {
            _gamepad?.RegisterButtonState(button, func, _unregister);
        }
        public void RegisterButtonEvent(GamePadButton button, ButtonInputType type, Action func)
        {
            _gamepad?.RegisterButtonEvent(button, type, func, _unregister);
        }
        public void RegisterButtonEvent(GamePadAxis button, ButtonInputType type, Action func)
        {
            _gamepad?.RegisterButtonEvent(button, type, func, _unregister);
        }
        public void RegisterAxisUpdate(GamePadAxis axis, DelAxisValue func, bool continuousUpdate)
        {
            _gamepad?.RegisterAxisUpdate(axis, func, continuousUpdate, _unregister);
        }
        #endregion

        public void UnregisterAll()
        {
            UnregisterAllFrom(_gamepad);
            UnregisterAllFrom(_keyboard);
            UnregisterAllFrom(_mouse);
        }
        public void UnregisterAllFrom(InputDevice device)
        {
            if (device == null)
                return;


        }
    }
}
