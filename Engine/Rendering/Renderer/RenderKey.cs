using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering
{
    //Larger enum numbers mean we want to render this first.
    public enum EDrawLayer : uint
    {
        Viewport = 1,
        FullScreenHUD = 0,
    }
    public enum EViewport : uint
    {
        Viewport0 = 3,
        Viewport1 = 2,
        Viewport2 = 1,
        Viewport3 = 0,
    }
    public enum EViewportLayer : uint
    {
        Sky = 3,
        World = 2,
        Hud2d = 1,
        Hud3d = 0,
    }
    public enum EPassType : uint
    {
        Opaque = 1,
        Translucent = 0
    }
    public enum ERenderCommand : uint
    {
        MultMatrix,
    }
    public class RenderKey
    {
        private Bin64 _data;
        
        public RenderKey(ulong data) { _data = data; }
        public RenderKey(
            EDrawLayer dlayer,
            EViewport viewport,
            EViewportLayer vlayer,
            EPassType translucencyPass,
            ERenderCommand command,
            uint commandData)
        {
            DrawLayer = dlayer;
            Viewport = viewport;
        }


        public void SetDepth(float depth, float nearDepth, float farDepth)
        {
            float percentDepth = depth / (farDepth - nearDepth);

        }

        public EDrawLayer DrawLayer
        {
            get { return (EDrawLayer)_data[63, 1]; }
            set { _data[63, 1] = (uint)value; }
        }
        public EViewport Viewport
        {
            get { return (EViewport)_data[61, 2]; }
            set { _data[61, 2] = (uint)value; }
        }
        public EViewportLayer ViewportLayer
        {
            get { return (EViewportLayer)_data[59, 2]; }
            set { _data[59, 2] = (uint)value; }
        }
        public EPassType Pass
        {
            get { return (EPassType)_data[58, 1]; }
            set { _data[58, 1] = (uint)value; }
        }
        public ushort Sequence
        {
            get { return (ushort)_data[42, 16]; }
            set { _data[42, 16] = value; }
        }
        public bool IsCommand
        {
            get { return _data[25]; }
            set { _data[25] = value; }
        }

        public ushort Depth { get { return (ushort)_data[24, 16]; } set { _data[24, 16] = value; } }
        public UInt24 MaterialId { get { return (UInt24)_data[0, 24]; } set { _data[0, 24] = value; } }

        private void SortArrayWithRadixSort()
        {
            int[] array = { 7, 5, 3, 6, 76, 45, 32 };
            int[] sortedArray = RadixSort(array);

            // PrintResult(sortedArray);
        }

        public int[] RadixSort(int[] array)
        {
            bool isFinished = false;
            int digitPosition = 0;

            List<Queue<int>> buckets = new List<Queue<int>>();
            InitializeBuckets(buckets);

            while (!isFinished)
            {
                isFinished = true;

                foreach (int value in array)
                {
                    int bucketNumber = GetBucketNumber(value, digitPosition);
                    if (bucketNumber > 0)
                    {
                        isFinished = false;
                    }

                    buckets[bucketNumber].Enqueue(value);
                }

                int i = 0;
                foreach (Queue<int> bucket in buckets)
                {
                    while (bucket.Count > 0)
                    {
                        array[i] = bucket.Dequeue();
                        i++;
                    }
                }

                digitPosition++;
            }

            return array;
        }

        private int GetBucketNumber(int value, int digitPosition)
        {
            int bucketNumber = (value / (int)Math.Pow(10, digitPosition)) % 10;
            return bucketNumber;
        }

        private static void InitializeBuckets(List<Queue<int>> buckets)
        {
            for (int i = 0; i < 10; i++)
            {
                Queue<int> q = new Queue<int>();
                buckets.Add(q);
            }
        }

        public static implicit operator RenderKey(ulong value) { return new RenderKey(value); }
        public static implicit operator ulong(RenderKey value) { return value._data; }
    }
}
