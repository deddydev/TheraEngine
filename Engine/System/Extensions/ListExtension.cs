using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public static class ListExtension
    {
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
