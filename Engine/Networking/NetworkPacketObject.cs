using System.Runtime.InteropServices;
using TheraEngine.Core.Maths;

namespace TheraEngine.Networking
{
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct StateData
    {
        public int ActorID;

    }
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct Transform
    {
        public Half PosX;
        public Half PosY;
        public Half PosZ;
    }
}
