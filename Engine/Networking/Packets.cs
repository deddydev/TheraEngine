using System.Runtime.InteropServices;
using TheraEngine.Core.Maths;

namespace TheraEngine.Networking
{
    public enum EPacketType : int
    {
        Invalid = 0,
        Input = 1,
        Transform = 2,
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
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct TPacketHeader
    {
        public EPacketType PacketType;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct TPacketInput
    {
        public TPacketHeader Header;
        public EDeviceType DeviceType;
        public EInputType InputType;
        public byte PlayerIndex;
        public byte InputIndex;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct TPacketPressedInput
    {
        public TPacketInput Header;
        public byte Pressed;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct TPacketAxisInput
    {
        public TPacketInput Header;
        public float Value;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct TPacketTransform
    {
        public TPacketHeader Header;
        public ushort ActorID;
        public Half PosX;
        public Half PosY;
        public Half PosZ;
    }
}
