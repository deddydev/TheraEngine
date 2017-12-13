using System;
using System.Collections.Generic;

namespace TheraEngine.Input.Devices
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
    public enum MouseMoveType
    {
        Relative,
        Absolute,
    }
    /// <summary>
    /// Input from controllers.
    /// </summary>
    public class InputInterface : TObject
    {
        //TODO: contain reference to owning local player controller
        //and use its player index enum instead
        //Also update devices when the player index is changed
        public int PlayerIndex
        {
            get => _playerIndex;
            set
            {
                _playerIndex = value;
                UpdateDevices();
            }
        }

        public BaseGamePad Gamepad => _gamepad;
        public BaseKeyboard Keyboard => _keyboard;
        public BaseMouse Mouse => _mouse;

        Dictionary<string, List<EKey>> _namedKeys = new Dictionary<string, List<EKey>>();
        Dictionary<string, List<GamePadButton>> _namedGamepadButtons = new Dictionary<string, List<GamePadButton>>();
        Dictionary<string, List<GamePadAxis>> _namedGamepadAxes = new Dictionary<string, List<GamePadAxis>>();
        Dictionary<string, List<EMouseButton>> _namedMouseButtons = new Dictionary<string, List<EMouseButton>>();

        public event DelWantsInputsRegistered WantsInputsRegistered;
        public static List<DelWantsInputsRegistered> GlobalRegisters = new List<DelWantsInputsRegistered>();

        private bool _unregister = false;
        private int _playerIndex;

        private BaseGamePad _gamepad;
        private BaseKeyboard _keyboard;
        private BaseMouse _mouse;

        public InputInterface(int playerIndex) { PlayerIndex = playerIndex; }

        internal void UpdateDevices()
        {
            TryUnregisterInput();
            GetDevices();
            TryRegisterInput();
        }
        internal void TryRegisterInput()
        {
            if (_gamepad != null || _keyboard != null || _mouse != null)
            {
                TryUnregisterInput();

                _unregister = false;
                //Interface gets input from pawn, hud, local controller, and global list
                WantsInputsRegistered?.Invoke(this);
                foreach (DelWantsInputsRegistered register in GlobalRegisters)
                    register(this);
            }
        }
        internal void TryUnregisterInput()
        {
            if (_gamepad != null || _keyboard != null || _mouse != null)
            {
                //Call for regular old input registration, but in the backend,
                //unregister all calls instead of registering them.
                //This way the user doesn't have to do any extra work
                //other than just registering the inputs.
                _unregister = true;
                WantsInputsRegistered?.Invoke(this);
                foreach (DelWantsInputsRegistered register in GlobalRegisters)
                    register(this);
                _unregister = false;
            }
        }
        private void GetDevices()
        {
            InputDevice[] gamepads = InputDevice.CurrentDevices[InputDeviceType.Gamepad];
            InputDevice[] keyboards = InputDevice.CurrentDevices[InputDeviceType.Keyboard];
            InputDevice[] mice = InputDevice.CurrentDevices[InputDeviceType.Mouse];

            if (_playerIndex >= 0 && _playerIndex < gamepads.Length)
                _gamepad = gamepads[_playerIndex] as BaseGamePad;

            //Keyboard and mouse are reserved for the first player only
            //TODO: support multiple mice and keyboard? Could get difficult with laptops and trackpads and whatnot. Probably no-go.
            //TODO: support input from ALL keyboards and mice for first player. Not just the first found keyboard and mouse.

            if (keyboards.Length > 0 && _playerIndex == 0)
                _keyboard = keyboards[0] as BaseKeyboard;

            if (mice.Length > 0 && _playerIndex == 0)
                _mouse = mice[0] as BaseMouse;
        }

        #region Mouse input registration
        public void RegisterButtonPressed(EMouseButton button, DelButtonState func, InputPauseType pauseType = InputPauseType.TickOnlyWhenUnpaused)
            => _mouse?.RegisterButtonPressed(button, pauseType, func, _unregister);
        public void RegisterButtonEvent(EMouseButton button, ButtonInputType type, Action func, InputPauseType pauseType = InputPauseType.TickOnlyWhenUnpaused)
            => _mouse?.RegisterButtonEvent(button, type, pauseType, func, _unregister);
        public void RegisterMouseScroll(DelMouseScroll func, InputPauseType pauseType = InputPauseType.TickOnlyWhenUnpaused)
            => _mouse?.RegisterScroll(func, pauseType, _unregister);
        public void RegisterMouseMove(DelCursorUpdate func, bool relative, InputPauseType pauseType = InputPauseType.TickOnlyWhenUnpaused)
            => _mouse?.RegisterMouseMove(func, pauseType, relative, _unregister);
        #endregion

        #region Keyboard input registration
        public void RegisterButtonPressed(EKey button, DelButtonState func, InputPauseType pauseType = InputPauseType.TickOnlyWhenUnpaused)
            => _keyboard?.RegisterButtonPressed(button, pauseType, func, _unregister);
        public void RegisterButtonEvent(EKey button, ButtonInputType type, Action func, InputPauseType pauseType = InputPauseType.TickOnlyWhenUnpaused)
            => _keyboard?.RegisterButtonEvent(button, type, pauseType, func, _unregister);
        #endregion

        #region Gamepad input registration
        public void RegisterButtonPressed(GamePadAxis axis, DelButtonState func, InputPauseType pauseType = InputPauseType.TickOnlyWhenUnpaused)
            => _gamepad?.RegisterButtonState(axis, pauseType, func, _unregister);
        public void RegisterButtonPressed(GamePadButton button, DelButtonState func, InputPauseType pauseType = InputPauseType.TickOnlyWhenUnpaused)
            => _gamepad?.RegisterButtonState(button, pauseType, func, _unregister);
        public void RegisterButtonEvent(GamePadButton button, ButtonInputType type, Action func, InputPauseType pauseType = InputPauseType.TickOnlyWhenUnpaused)
            => _gamepad?.RegisterButtonEvent(button, type, pauseType, func, _unregister);
        public void RegisterButtonEvent(GamePadAxis button, ButtonInputType type, Action func, InputPauseType pauseType = InputPauseType.TickOnlyWhenUnpaused)
            => _gamepad?.RegisterButtonEvent(button, type, pauseType, func, _unregister);
        public void RegisterAxisUpdate(GamePadAxis axis, DelAxisValue func, bool continuousUpdate, InputPauseType pauseType = InputPauseType.TickOnlyWhenUnpaused)
            => _gamepad?.RegisterAxisUpdate(axis, pauseType, func, continuousUpdate, _unregister);
        #endregion
    }
}
