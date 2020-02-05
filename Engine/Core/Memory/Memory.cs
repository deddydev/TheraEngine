using System.IO;
using System;

namespace TheraEngine.Core.Memory
{
    public static class Memory
    {
        public static bool MacOSXCheck { get; }

        public static void Move(VoidPtr dst, VoidPtr src, uint size)
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    Win32.MoveMemory(dst, src, size);
                    break;
                case PlatformID.MacOSX:
                    OSX.memmove(dst, src, size);
                    break;
                case PlatformID.Unix:
                    if (Directory.Exists("/Applications")
                        & Directory.Exists("/System")
                        & Directory.Exists("/Users")
                        & Directory.Exists("/Volumes"))
                        goto case PlatformID.MacOSX;
                    else
                        Linux.memmove(dst, src, size);
                    break;
                default:
                    Engine.LogWarning(Environment.OSVersion.Platform.ToString() + " not supported.");
                    break;
            }
        }

        public static void Fill(VoidPtr dest, uint length, byte value)
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                        Win32.FillMemory(dest, length, value);
                        break;
                case PlatformID.MacOSX:
                        OSX.memset(dest, value, length);
                        break;
                case PlatformID.Unix:
                        if (Directory.Exists("/Applications")
                            & Directory.Exists("/System")
                            & Directory.Exists("/Users")
                            & Directory.Exists("/Volumes"))
                            goto case PlatformID.MacOSX;
                        else
                            Linux.memset(dest, value, length);
                        break;
                default:
                    Engine.LogWarning(Environment.OSVersion.Platform.ToString() + " not supported.");
                    break;
            }
        }
    }
}
