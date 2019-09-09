using System.Runtime.InteropServices;
using TheraEngine.Core.Maths;

namespace TheraEngine.Networking
{
    public enum EPacketType : byte
    {
        Invalid         = 0,
        Input           = 1,
        Connection      = 2,
        State           = 3,
    }
    public enum EConnectionMessage : byte
    {
        Request                 = 0,
        Accepted                = 1,
        Denied                  = 2,
        LocalPlayerCountChanged = 3,
    }
    public enum EStateType : byte
    {
        Invalid  = 0,
        World    = 1,
        GameMode = 2,
    }
    public enum EInputType : byte
    {
        Invalid                 = 0,
        ButtonPressedState      = 1,
        ButtonAction            = 2,
        AxisButtonPressedState  = 3,
        AxisButtonAction        = 4,
        AxisValue               = 5,
    }
    public enum EDeviceType : byte
    {
        Invalid     = 0,
        Gamepad     = 1,
        Keyboard    = 2,
        Mouse       = 3,
    }
    public enum EButtonActionType : byte
    {
        Invalid = 0,
        Pressed,
        Released,
        DoubleClicked,
        Held,
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TPacketHeader
    {
        public EPacketType PacketType;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TPacketConnection
    {
        public TPacketHeader Header;
        public EConnectionMessage ConnectionMessage;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TPacketConnectionAccepted
    {
        public TPacketConnection Header;
        public byte ServerIndex;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TPacketInput
    {
        public TPacketHeader Header;
        public EDeviceType DeviceType;
        public EInputType InputType;
        public byte PlayerIndex;
        public byte InputIndex;
        public byte ListIndex;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TPacketPressedInput
    {
        public TPacketInput Header;
        public byte Pressed;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TPacketAxisInput
    {
        public TPacketInput Header;
        public float Value;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TPacketTransform
    {
        public TPacketHeader Header;
        public ushort ActorID;
        public Half PosX;
        public Half PosY;
        public Half PosZ;
    }
}
