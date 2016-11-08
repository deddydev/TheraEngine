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
        public RenderKey(
            EDrawLayer dlayer,
            EViewport viewport,
            EViewportLayer vlayer,
            EPassType translucencyPass,
            float depth,
            uint materialId)
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
            get { return (EPassType)_data[57, 2]; }
            set { _data[57, 2] = (uint)value; }
        }
        public bool IsCommand
        {
            get { return _data[56]; }
            set { _data[56] = value; }
        }
        public ushort Sequence
        {
            get { return (ushort)_data[40, 16]; }
            set { _data[40, 16] = value; }
        }
        public UInt24 Depth
        {
            get { return (UInt24)_data[24, 20]; }
            set { _data[40, 16] = value; }
        }
        public UInt24 MaterialId { get { return (UInt24)_data[0, 24]; } set { _data[0, 24] = value; } }

        public static ulong[] RadixSort(ulong[] array)
        {
            int id = Engine.StartDebugTimer();
            Queue<ulong>[] buckets = new Queue<ulong>[15];
            for (int i = 0; i < 0xF; i++)
                buckets[i] = new Queue<ulong>();
            for (int i = 60; i >= 0; i -= 4)
            {
                foreach (ulong value in array)
                    buckets[(int)((value >> i) & 0xF)].Enqueue(value);
                int x = 0;
                foreach (Queue<ulong> bucket in buckets)
                    while (bucket.Count > 0)
                        array[x++] = bucket.Dequeue();
            }
            Engine.EndDebugTimer(id, "Radix Sort Time: ");
            return array;
        }
        public static implicit operator RenderKey(ulong value) { return new RenderKey(value); }
        public static implicit operator ulong(RenderKey value) { return value._data; }
    }
}
