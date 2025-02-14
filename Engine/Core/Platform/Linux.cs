﻿using System.Runtime.InteropServices;
using TheraEngine.Core.Memory;

namespace System
{
    public static unsafe class Linux
    {
        public const string libmName = "libm.so";

        [DllImport(libmName)]
        public static extern void memset(VoidPtr dest, byte value, uint length);
        [DllImport(libmName)]
        public static extern void memmove(VoidPtr dst, VoidPtr src, uint length);

        [DllImport(libmName)]
        public static extern VoidPtr mmap(VoidPtr addr, uint len, MMapProtect prot, MMapFlags flags, int fildes, uint off);
        [DllImport(libmName)]
        public static extern int munmap(VoidPtr addr, uint len);

        [Flags]
        public enum MMapProtect : int
        {
            None = 0x00,
            Read = 0x01,
            Write = 0x02,
            Execute = 0x04
        }

        [Flags]
        public enum MMapFlags : int
        {
            Shared = 0x01,
            Private = 0x02,
            Fixed = 0x10
        }
    }
}
