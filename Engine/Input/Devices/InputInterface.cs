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
        Unbounded,
    }
    /// <summary>
    /// Handles input from keyboards, mice, gamepads, etc.
    /// </summary>
    public class InputInterface : TObject
    {
        /// <summary>
        /// Global registration methods found here are called to register input for any and all controllers,
        /// regardless of the pawn they control or the type of controller they are.
        /// </summary>
        public static List<DelWantsInputsRegistered> GlobalRegisters = new List<DelWantsInputsRegistered>();

        public event DelWantsInputsRegistered WantsInputsRegistered;

        public InputInterface(int localPlayerIndex)
            => LocalPlayerIndex = localPlayerIndex;

        public BaseGamePad Gamepad { get; private set; }
        public BaseKeyboard Keyboard { get; private set; }
        public BaseMouse Mouse { get; private set; }

        //TODO: contain reference to owning local player controller
        //and use its player index enum instead
        //Also update devices when the player index is changed
        public int LocalPlayerIndex
        {
            get => _playerIndex;
            set
            {
                _playerIndex = value;
                UpdateDevices();
            }
        }
        private int _playerIndex;
        private bool _unregister = false;

        //Dictionary<string, List<EKey>> _namedKeys = new Dictionary<string, List<EKey>>();
        //Dictionary<string, List<GamePadButton>> _namedGamepadButtons = new Dictionary<string, List<GamePadButton>>();
        //Dictionary<string, List<GamePadAxis>> _namedGamepadAxes = new Dictionary<string, List<GamePadAxis>>();
        //Dictionary<string, List<EMouseButton>> _namedMouseButtons = new Dictionary<string, List<EMouseButton>>();

        internal void UpdateDevices()
        {
            TryUnregisterInput();
            GetDevices();
            TryRegisterInput();
        }
        internal void TryRegisterInput()
        {
            if (Gamepad != null || Keyboard != null || Mouse != null)
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
            if (Gamepad != null || Keyboard != null || Mouse != null)
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
            AttachInterfaceToDevices(true);

            InputDevice[] gamepads = InputDevice.CurrentDevices[InputDeviceType.Gamepad];
            InputDevice[] keyboards = InputDevice.CurrentDevices[InputDeviceType.Keyboard];
            InputDevice[] mice = InputDevice.CurrentDevices[InputDeviceType.Mouse];

            if (_playerIndex >= 0 && _playerIndex < gamepads.Length)
            {
                Gamepad = gamepads[_playerIndex] as BaseGamePad;
            }

            //Keyboard and mouse are reserved for the first player only
            //TODO: support multiple mice and keyboard? Could get difficult with laptops and trackpads and whatnot. Probably no-go.
            //TODO: support input from ALL keyboards and mice for first player. Not just the first found keyboard and mouse.

            if (keyboards.Length > 0 && _playerIndex == 0)
            {
                Keyboard = keyboards[0] as BaseKeyboard;
            }

            if (mice.Length > 0 && _playerIndex == 0)
            {
                Mouse = mice[0] as BaseMouse;
            }

            AttachInterfaceToDevices(false);
        }

        private void AttachInterfaceToDevices(bool detach)
        {
            if (detach)
            {
                if (Gamepad != null)
                {
                    Gamepad.InputInterface = null;
                }
                if (Keyboard != null)
                {
                    Keyboard.InputInterface = null;
                }
                if (Mouse != null)
                {
                    Mouse.InputInterface = null;
                }
            }
            else
            {
                if (Gamepad != null)
                {
                    Gamepad.InputInterface = this;
                }
                if (Keyboard != null)
                {
                    Keyboard.InputInterface = this;
                }
                if (Mouse != null)
                {
                    Mouse.InputInterface = this;
                }
            }
        }

        #region Mouse input registration
        public void RegisterButtonPressed(EMouseButton button, DelButtonState func, EInputPauseType pauseType = EInputPauseType.TickOnlyWhenUnpaused)
            => Mouse?.RegisterButtonPressed(button, pauseType, func, _unregister);
        public void RegisterButtonEvent(EMouseButton button, ButtonInputType type, Action func, EInputPauseType pauseType = EInputPauseType.TickOnlyWhenUnpaused)
            => Mouse?.RegisterButtonEvent(button, type, pauseType, func, _unregister);
        public void RegisterMouseScroll(DelMouseScroll func, EInputPauseType pauseType = EInputPauseType.TickOnlyWhenUnpaused)
            => Mouse?.RegisterScroll(func, pauseType, _unregister);
        public void RegisterMouseMove(DelCursorUpdate func, MouseMoveType type, EInputPauseType pauseType = EInputPauseType.TickOnlyWhenUnpaused)
            => Mouse?.RegisterMouseMove(func, pauseType, type, _unregister);
        #endregion

        #region Keyboard input registration
        public void RegisterButtonPressed(EKey button, DelButtonState func, EInputPauseType pauseType = EInputPauseType.TickOnlyWhenUnpaused)
            => Keyboard?.RegisterButtonPressed(button, pauseType, func, _unregister);
        public void RegisterButtonEvent(EKey button, ButtonInputType type, Action func, EInputPauseType pauseType = EInputPauseType.TickOnlyWhenUnpaused)
            => Keyboard?.RegisterButtonEvent(button, type, pauseType, func, _unregister);
        #endregion

        #region Gamepad input registration
        public void RegisterButtonPressed(GamePadAxis axis, DelButtonState func, EInputPauseType pauseType = EInputPauseType.TickOnlyWhenUnpaused)
            => Gamepad?.RegisterButtonState(axis, pauseType, func, _unregister);
        public void RegisterButtonPressed(GamePadButton button, DelButtonState func, EInputPauseType pauseType = EInputPauseType.TickOnlyWhenUnpaused)
            => Gamepad?.RegisterButtonState(button, pauseType, func, _unregister);
        public void RegisterButtonEvent(GamePadButton button, ButtonInputType type, Action func, EInputPauseType pauseType = EInputPauseType.TickOnlyWhenUnpaused)
            => Gamepad?.RegisterButtonEvent(button, type, pauseType, func, _unregister);
        public void RegisterButtonEvent(GamePadAxis button, ButtonInputType type, Action func, EInputPauseType pauseType = EInputPauseType.TickOnlyWhenUnpaused)
            => Gamepad?.RegisterButtonEvent(button, type, pauseType, func, _unregister);
        public void RegisterAxisUpdate(GamePadAxis axis, DelAxisValue func, bool continuousUpdate, EInputPauseType pauseType = EInputPauseType.TickOnlyWhenUnpaused)
            => Gamepad?.RegisterAxisUpdate(axis, pauseType, func, continuousUpdate, _unregister);
        #endregion
    }
}
