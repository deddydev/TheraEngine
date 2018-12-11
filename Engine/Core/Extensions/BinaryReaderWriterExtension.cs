using System.IO;
using System.Runtime.InteropServices;

namespace System
{
    public static class BinaryReaderExtension
    {
        public unsafe static byte[] ReadBytes(this FileStream stream, int count)
        {
            byte[] bytes = new byte[count];
            for (int i = 0; i < count; ++i)
                bytes[i] = (byte)stream.ReadByte();
            return bytes;
        }
        public unsafe static T Read<T>(this FileStream reader) where T : unmanaged
        {
            int size = sizeof(T);
            byte[] bytes = reader.ReadBytes(size);
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T result = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            handle.Free();
            return result;
        }
        public unsafe static void WriteBytes(this FileStream stream, byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; ++i)
                stream.WriteByte(bytes[i]);
        }
        public unsafe static void Write<T>(this FileStream writer, T value) where T : unmanaged
        {
            int size = sizeof(T);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(value, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
        }
    }
}
