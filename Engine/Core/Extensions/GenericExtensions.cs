using System;
using System.Runtime.InteropServices;

namespace TheraEngine.Core.Extensions
{
    public static class GenericExtensions
    {
        /// <summary>
        /// Converts an unmanaged structure to an array of bytes.
        /// </summary>
        public static unsafe byte[] ToByteArray<T>(this T data) where T : unmanaged
        {
            byte[] dataArr = new byte[sizeof(T)];
            void* addr = &data;
            Marshal.Copy((IntPtr)addr, dataArr, 0, dataArr.Length);
            return dataArr;
        }
        /// <summary>
        /// Converts an array of bytes to an unmanaged structure.
        /// </summary>
        public static unsafe T ToStruct<T>(this byte[] data) where T : unmanaged
        {
            T value = new T();
            void* addr = &value;
            Marshal.Copy(data, 0, (IntPtr)addr, Math.Min(data.Length, sizeof(T)));
            return value;
        }
        public static bool IsBoxed<T>(this T value)
        {
            return
                (typeof(T).IsInterface || typeof(T) == typeof(object)) &&
                value != null &&
                value.GetType().IsValueType;
        }
    }
}
