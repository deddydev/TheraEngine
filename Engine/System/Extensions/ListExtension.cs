namespace System.Collections.Generic
{
    public static class ListExtension
    {
        /// <summary>
        /// Returns true if index >= 0 && index < length.
        /// Use this so you don't have to write that every time.
        /// </summary>
        public static bool IndexInRange(this IList a, int value)
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
    }
}
