namespace System.Collections.Generic
{
    public static class ListExtension
    {
        public static Type DetermineElementType(this Type listType)
        {
            Type elementType = listType.GetElementType();
            if (elementType != null)
                return elementType;
            if (listType.IsGenericType && listType.GenericTypeArguments.Length == 1)
                return listType.GenericTypeArguments[0];
            return null;
        }
        public static Type DetermineElementType(this IList list)
        {
            Type listType = list.GetType();
            Type elementType = listType.GetElementType();
            if (elementType != null)
                return elementType;
            if (listType.IsGenericType && listType.GenericTypeArguments.Length == 1)
                return listType.GenericTypeArguments[0];
            return null;
        }
        public static Type DetermineKeyType(this IDictionary dic)
        {
            Type listType = dic.GetType();
            if (listType.IsGenericType)
                return listType.GenericTypeArguments[0];
            return null;
        }
        public static Type DetermineValueType(this IDictionary dic)
        {
            Type listType = dic.GetType();
            if (listType.IsGenericType)
                return listType.GenericTypeArguments[1];
            return null;
        }
        ///// <summary>
        ///// Returns true if index >= 0 && index<count
        ///// Use this so you don't have to write that every time.
        ///// </summary>
        //public static bool IndexInRange(this IList a, int value)
        //{
        //    return value >= 0 && value < a.Count;
        //}
        /// <summary>
        /// Returns true if index >= 0 && index < count
        /// Use this so you don't have to write that every time.
        /// </summary>
        public static bool IndexInRange<T>(this IList<T> a, int value)
        {
            return value >= 0 && value < a.Count;
        }
        public static void RadixSort(this List<uint> array)
        {
            //int id = Engine.StartTimer();
            Queue<uint>[] buckets = new Queue<uint>[15];
            for (int i = 0; i < 0xF; i++)
                buckets[i] = new Queue<uint>();
            for (int i = 60; i >= 0; i -= 4)
            {
                foreach (uint value in array)
                    buckets[(int)((value >> i) & 0xF)].Enqueue(value);
                int x = 0;
                foreach (Queue<uint> bucket in buckets)
                    while (bucket.Count > 0)
                        array[x++] = bucket.Dequeue();
            }
            //float seconds = Engine.EndTimer(id);
            //Engine.DebugPrint("Radix Sort took " + seconds + " seconds.");
        }
        public static IList<T> FillWith<T>(this IList<T> list, T value)
        {
            for (int i = 0; i < list.Count; i++)
                list[i] = value;

            return list;
        }
        public static IList<T> FillWith<T>(this IList<T> list, Func<int, T> factory)
        {
            if (factory == null)
                return list;

            for (int i = 0; i < list.Count; i++)
                list[i] = factory(i);

            return list;
        }
    }
}
