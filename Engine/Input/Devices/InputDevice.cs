using System;
using System.Collections.Generic;
using TheraEngine.Networking;
using TheraEngine.Rendering;

namespace TheraEngine.Input.Devices
{
    public enum EInputPauseType
    {
        TickAlways              = 0,
        TickOnlyWhenUnpaused    = 1,
        TickOnlyWhenPaused      = 2,
    }
    [Serializable]
    public abstract class InputDevice : TObject
    {
        //TODO: mouse and keyboard should just be their own global devices for ALL input from ANY mice or keyboards
        public static Dictionary<EInputDeviceType, InputDevice[]> CurrentDevices =
            new Dictionary<EInputDeviceType, InputDevice[]>()
        {
            { EInputDeviceType.Gamepad,  new InputDevice[4] },
            { EInputDeviceType.Keyboard, new InputDevice[4] },
            { EInputDeviceType.Mouse,    new InputDevice[4] },
        };

        protected ButtonManager[] _buttonStates;
        protected AxisManager[] _axisStates;

        protected int _index;
        protected bool _isConnected;

        public ConnectedStateChange ConnectionStateChanged;

        public bool IsConnected => _isConnected;
        public int Index => _index;
        public abstract EDeviceType DeviceType { get; }
        public InputInterface InputInterface { get; internal set; }

        protected InputDevice(int index)
        {
            _index = index;
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
            if (_isConnected != isConnected)
            {
                _isConnected = isConnected;
                ConnectionStateChanged?.Invoke(_isConnected);
            }

            //TODO: only tick inputs for local controllers that have registered input to the currently focused render panel
            return _isConnected && RenderContext.Focused != null;
        }
        public static void RegisterButtonEvent(ButtonManager m, EButtonInputType type, EInputPauseType pauseType, Action func, bool unregister)
        {
            m?.Register(func, type, pauseType, unregister);
        }

        private bool CanSend()
            => Engine.Network != null &&
            !Engine.Network.IsServer &&
            InputInterface != null;
            //&&
            //Engine.LocalPlayers.IndexInRange(InputInterface.LocalPlayerIndex);
        
        protected void SendButtonAction(int buttonIndex, int listIndex)
        {
            if (!CanSend())
                return;

            TPacketInput packet = new TPacketInput();
            packet.Header.PacketType = EPacketType.Input;
            packet.DeviceType = DeviceType;
            packet.InputType = EInputType.ButtonAction;
            packet.InputIndex = (byte)buttonIndex;
            //packet.PlayerIndex = (byte)Engine.LocalPlayers[InputInterface.LocalPlayerIndex].ServerPlayerIndex;

            Engine.Network.SendPacket(packet);
        }
        protected void SendButtonPressedState(int buttonIndex, int listIndex, bool pressed)
        {
            if (!CanSend())
                return;

            TPacketPressedInput packet = new TPacketPressedInput();
            packet.Header.Header.PacketType = EPacketType.Input;
            packet.Header.DeviceType = DeviceType;
            packet.Header.InputType = EInputType.ButtonPressedState;
            packet.Header.InputIndex = (byte)buttonIndex;
            //packet.Header.PlayerIndex = (byte)Engine.LocalPlayers[InputInterface.LocalPlayerIndex].ServerPlayerIndex;
            packet.Pressed = (byte)(pressed ? 1 : 0);

            Engine.Network.SendPacket(packet);
        }
        protected void SendAxisButtonAction(int axisIndex, int listIndex)
        {
            if (!CanSend())
                return;

            TPacketInput packet = new TPacketInput();
            packet.Header.PacketType = EPacketType.Input;
            packet.DeviceType = DeviceType;
            packet.InputType = EInputType.AxisButtonAction;
            packet.InputIndex = (byte)axisIndex;
            //packet.PlayerIndex = (byte)Engine.LocalPlayers[InputInterface.LocalPlayerIndex].ServerPlayerIndex;

            Engine.Network.SendPacket(packet);
        }
        protected void SendAxisButtonPressedState(int axisIndex, int listIndex, bool pressed)
        {
            if (!CanSend())
                return;

            TPacketPressedInput packet = new TPacketPressedInput();
            packet.Header.Header.PacketType = EPacketType.Input;
            packet.Header.DeviceType = DeviceType;
            packet.Header.InputType = EInputType.AxisButtonPressedState;
            packet.Header.InputIndex = (byte)axisIndex;
            //packet.Header.PlayerIndex = (byte)Engine.LocalPlayers[InputInterface.LocalPlayerIndex].ServerPlayerIndex;
            packet.Pressed = (byte)(pressed ? 1 : 0);

            Engine.Network.SendPacket(packet);
        }
        protected void SendAxisValue(int axisIndex, int listIndex, float value)
        {
            if (!CanSend())
                return;

            TPacketAxisInput packet = new TPacketAxisInput();
            packet.Header.Header.PacketType = EPacketType.Input;
            packet.Header.DeviceType = DeviceType;
            packet.Header.InputType = EInputType.AxisValue;
            packet.Header.InputIndex = (byte)axisIndex;
            //packet.Header.PlayerIndex = (byte)Engine.LocalPlayers[InputInterface.LocalPlayerIndex].ServerPlayerIndex;
            packet.Value = value;

            Engine.Network.SendPacket(packet);
        }
    }
}
