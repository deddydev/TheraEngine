using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomEngine.Input.Devices
{
    public enum InputType
    {
        Pressed,
        Released,
        Held,
        DoublePressed,
        AxisUpdate,
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

        public event DelWantsInputsRegistered WantsInputsRegistered;
        
        private int _playerIndex;
        private List<InputDevice> _devices = new List<InputDevice>();

        public InputInterface(int playerIndex) { _playerIndex = playerIndex; }

        public void UpdateDevices()
        {
            UnregisterAll();
            _devices = GetDevices();
            WantsInputsRegistered?.Invoke(this);
        }
        public List<InputDevice> GetDevices()
        {
            //List<InputDevice> devices = new List<InputDevice>();
            //var gamepads = InputDevice.CurrentDevices[InputDeviceType.Gamepad];
            //if (gamepads != null && _playerIndex < gamepads.Count)
            //    devices.Add(gamepads[_playerIndex]);
            //var keyboards = InputDevice.CurrentDevices[InputDeviceType.Keyboard];
            //if (keyboards != null && _playerIndex < keyboards.Count)
            //    devices.Add(keyboards[_playerIndex]);
            //var mice = InputDevice.CurrentDevices[InputDeviceType.Mouse];
            //if (mice != null && _playerIndex < mice.Count)
            //    devices.Add(mice[_playerIndex]);
            //return devices;

            return InputDevice.CurrentDevices.Where(x => _playerIndex < x.Value.Count).Select(x => x.Value[_playerIndex]).ToList();
        }
        public void Register(string inputName, InputType type, Action func)
        {

        }
        public void Register(string inputName, InputType type, Action<float> func)
        {

        }
        public void Register(EKey key, InputType type, Action func)
        {

        }
        public void Register(EKey key, InputType type, Action<float> func)
        {

        }
        public void Register(GamePadButton button, InputType type, Action func)
        {

        }
        public void Register(GamePadButton button, InputType type, Action<float> func)
        {

        }
        public void Register(GamePadAxis axis, InputType type, Action func)
        {
            
        }
        public void Register(GamePadAxis axis, InputType type, Action<float> func)
        {

        }
        public void UnregisterAll()
        {
            foreach (InputDevice device in _devices)
                UnregisterAllFrom(device);
        }
        public void UnregisterAllFrom(InputDevice device)
        {

        }
    }
}
