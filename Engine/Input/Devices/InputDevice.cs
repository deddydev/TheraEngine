using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Input.Devices
{
    public enum InputPauseType
    {
        TickAlways              = 0,
        TickOnlyWhenUnpaused    = 1,
        TickOnlyWhenPaused      = 2,
    }
    public abstract class InputDevice : ObjectBase
    {
        //TODO: mouse and keyboard should just be their own global devices for ALL input from ANY mice or keyboards
        public static Dictionary<InputDeviceType, InputDevice[]> CurrentDevices =
            new Dictionary<InputDeviceType, InputDevice[]>()
        {
            { InputDeviceType.Gamepad,  new InputDevice[4] },
            { InputDeviceType.Keyboard, new InputDevice[4] },
            { InputDeviceType.Mouse,    new InputDevice[4] },
        };

        protected ButtonManager[] _buttonStates;
        protected AxisManager[] _axisStates;

        protected int _index;
        protected bool _isConnected;

        public ConnectedStateChange ConnectionStateChanged;

        public bool IsConnected => _isConnected;
        public int Index => _index;

        public InputDevice(int index)
        {
            _index = index;
            //Debug.WriteLine(GetType().ToString() + _index + " created.");
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Input, UpdateStates);
            ResetStates();
        }
        protected abstract int GetButtonCount();
        protected abstract int GetAxisCount();
        private void ResetStates()
        {
            _buttonStates = new ButtonManager[GetButtonCount()];
            _axisStates = new AxisManager[GetAxisCount()];
        }
        protected abstract void UpdateStates(float delta);
        /// <summary>
        /// Returns true if connected.
        /// </summary>
        protected bool UpdateConnected(bool isConnected)
        {
            if (_isConnected == isConnected)
                return _isConnected;
            _isConnected = isConnected;
            ConnectionStateChanged?.Invoke(_isConnected);
            return _isConnected;
        }
        public static void RegisterButtonEvent(ButtonManager m, ButtonInputType type, InputPauseType pauseType, Action func, bool unregister)
        {
            m?.Register(func, type, pauseType, unregister);
        }
    }
}
