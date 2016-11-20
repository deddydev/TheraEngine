using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomEngine.Input.Devices
{
    public enum ButtonInputType
    {
        Pressed,
        Released,
        Held,
        DoublePressed,
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
                WantsInputsRegistered?.Invoke(this);
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
            _mouse?.RegisterButtonPressed(button, func);
        }
        public void RegisterButtonEvent(EMouseButton button, ButtonInputType type, Action func)
        {
            _mouse?.RegisterButtonEvent(button, type, func);
        }
        public void RegisterMouseScroll(DelMouseScroll func)
        {
            _mouse?.RegisterScroll(func);
        }
        public void RegisterMouseMove(DelCursorUpdate func, bool relative)
        {
            _mouse?.RegisterMouseMove(func, relative);
        }
        #endregion

        #region Keyboard input registration
        public void RegisterButtonPressed(EKey button, Action<bool> func)
        {
            _keyboard?.RegisterButtonPressed(button, func);
        }
        public void RegisterButtonEvent(EKey button, ButtonInputType type, Action func)
        {
            _keyboard?.RegisterButtonEvent(button, type, func);
        }
        #endregion

        #region Gamepad input registration
        public void RegisterButtonPressed(GamePadAxis axis, DelButtonState func)
        {
            _gamepad?.RegisterButtonState(axis, func);
        }
        public void RegisterButtonPressed(GamePadButton button, DelButtonState func)
        {
            _gamepad?.RegisterButtonState(button, func);
        }
        public void RegisterButtonEvent(GamePadButton button, ButtonInputType type, Action func)
        {
            _gamepad?.RegisterButtonEvent(button, type, func);
        }
        public void RegisterButtonEvent(GamePadAxis button, ButtonInputType type, Action func)
        {
            _gamepad?.RegisterButtonEvent(button, type, func);
        }
        public void RegisterAxisUpdate(GamePadAxis axis, DelAxisValue func, bool continuousUpdate)
        {
            _gamepad?.RegisterAxisUpdate(axis, func, continuousUpdate);
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
