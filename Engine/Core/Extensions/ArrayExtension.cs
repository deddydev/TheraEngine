using System.Collections.Generic;

namespace System
{
    public static unsafe class ArrayExtension
    {
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
        public static void FillWith<T>(this T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = value;
        }
        public static void FillWith<T>(this T[] array, Func<int, T> factory)
        {
            if (factory == null)
                return;

            for (int i = 0; i < array.Length; i++)
                array[i] = factory(i);
        }
        public static void ForEach<T>(this T[] data, Action<T> method)
            => Array.ForEach(data, method);
    }
}
