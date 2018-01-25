using System.Collections.Generic;

namespace System
{
    public static unsafe class ArrayExtension
    {
        /// <summary>
        /// Returns true if index >= 0 && index is less than length.
        /// Use this so you don't have to write that every time.
        /// </summary>
        public static bool IndexInArrayRange(this Array a, int value)
        {
            return value >= 0 && value < a.Length;
        }
        public static int[] FindAllOccurences(this Array a, object o)
        {
            List<int> l = new List<int>();
            int i = 0;
            foreach (object x in a)
            {
                if (x == o)
                    l.Add(i);
                i++;
            }
            return l.ToArray();
        }
        public static int IndexOf(this Array a, object value)
        {
            return Array.IndexOf(a, value);
        }
        public static bool Contains(this string[] a, string value, StringComparison comp)
        {
            for (int i = 0; i < a.Length; ++i)
                if (string.Equals(a[i], value, comp))
                    return true;
            return false;
        }
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
        public static T[] Append<T>(this T[] data, T[] appended)
        {
            T[] final = new T[data.Length + appended.Length];
            data.CopyTo(final, 0);
            appended.CopyTo(final, data.Length);
            return final;
        }
        public static void Resize<T>(this T[] data, int newSize)
        {
            Array.Resize(ref data, newSize);
        }
        public static T[] FillWith<T>(this T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = value;

            return array;
        }
        public static T[] FillWith<T>(this T[] array, Func<int, T> factory)
        {
            if (factory == null)
                return array;

            for (int i = 0; i < array.Length; i++)
                array[i] = factory(i);

            return array;
        }
        public static void ForEach<T>(this T[] data, Action<T> method)
            => Array.ForEach(data, method);
    }
}
