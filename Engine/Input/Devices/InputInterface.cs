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
        
        
        public void RegisterMouseMove(Action<float, float> func, MouseMoveType type)
        {
            throw new NotImplementedException();
        }

        Dictionary<string, List<EKey>> _namedKeys = new Dictionary<string, List<EKey>>();
        Dictionary<string, List<GamePadButton>> _namedGamepadButtons = new Dictionary<string, List<GamePadButton>>();
        Dictionary<string, List<GamePadAxis>> _namedGamepadAxes = new Dictionary<string, List<GamePadAxis>>();
        Dictionary<string, List<EMouseButton>> _namedMouseButtons = new Dictionary<string, List<EMouseButton>>();

        public event DelWantsInputsRegistered WantsInputsRegistered;
        
        private int _playerIndex;
        private Gamepad _gamepad;
        private Keyboard _keyboard;
        private Mouse _mouse;

        public InputInterface(int playerIndex)
        {
            _playerIndex = playerIndex;
            UpdateDevices();
        }

        public void UpdateDevices()
        {
            UnregisterAll();
            GetDevices();
            WantsInputsRegistered?.Invoke(this);
        }
        private void GetDevices()
        {
            var gamepads = InputDevice.CurrentDevices[InputDeviceType.Gamepad];
            if (gamepads != null && _playerIndex < gamepads.Count)
                _gamepad = (Gamepad)gamepads[_playerIndex];

            var keyboards = InputDevice.CurrentDevices[InputDeviceType.Keyboard];
            if (keyboards != null && _playerIndex < keyboards.Count)
                _keyboard = (Keyboard)keyboards[_playerIndex];

            var mice = InputDevice.CurrentDevices[InputDeviceType.Mouse];
            if (mice != null && _playerIndex < mice.Count)
                _mouse = (Mouse)mice[_playerIndex];
        }

        #region Mouse input registration
        public void RegisterButtonPressed(EMouseButton button, Action<bool> func)
        {
            _mouse?.RegisterButtonPressed(button, func);
        }
        public void RegisterButtonEvent(EMouseButton button, ButtonInputType type, Action func)
        {
            _mouse?.RegisterButtonEvent(button, type, func);
        }
        public void RegisterMouseScroll(Action<bool> func)
        {
            _mouse?.RegisterScroll(func);
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
        public void RegisterButtonPressed(GamePadAxis axis, Action<bool> func)
        {
            _gamepad?.RegisterButtonPressed(axis, func);
        }
        public void RegisterButtonPressed(GamePadButton button, Action<bool> func)
        {
            _gamepad?.RegisterButtonPressed(button, func);
        }
        public void RegisterButtonEvent(GamePadButton button, ButtonInputType type, Action func)
        {
            _gamepad?.RegisterButtonEvent(button, type, func);
        }
        public void RegisterButtonEvent(GamePadAxis button, ButtonInputType type, Action func)
        {
            _gamepad?.RegisterButtonEvent(button, type, func);
        }
        public void RegisterAxisUpdate(GamePadAxis axis, Action<float> func, bool continuousUpdate)
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
