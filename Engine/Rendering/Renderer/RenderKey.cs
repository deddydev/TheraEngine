using CustomEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering
{
    //Larger enum numbers mean we want to render this first.
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
    //public enum ERenderCommand : uint
    //{
    //    MultMatrix,
    //    SetCurrentViewport,
    //}
    public class RenderKey
    {
        private Bin32 _data;
        
        public RenderKey(uint data) { _data = data; }
        //public RenderKey(
        //    EDrawLayer dlayer,
        //    EViewport viewport,
        //    EViewportLayer vlayer,
        //    EPassType translucencyPass,
        //    ERenderCommand command,
        //    uint commandData)
        //{
        //    DrawLayer = dlayer;
        //    Viewport = viewport;
        //}
        public RenderKey(
            EViewportLayer vlayer,
            EPassType translucencyPass,
            float depth)
        {
            ViewportLayer = vlayer;
            Pass = translucencyPass;
            IsCommand = false;
            Depth = (uint)(depth * 0xFFFFFF);
        }
        public EViewportLayer ViewportLayer
        {
            get { return (EViewportLayer)_data[27, 2]; }
            set { _data[27, 2] = (uint)value; }
        }
        public EPassType Pass
        {
            get { return (EPassType)_data[25, 2]; }
            set { _data[25, 2] = (uint)value; }
        }
        public bool IsCommand
        {
            get { return _data[24]; }
            set { _data[24] = value; }
        }
        public ushort Sequence
        {
            get { return (ushort)_data[0, 24]; }
            set { _data[0, 24] = value; }
        }
        public UInt24 Depth
        {
            get { return _data[0, 24]; }
            set { _data[0, 24] = value; }
        }
        public static void RadixSort(ref List<uint> array)
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
        public override int GetHashCode()
        {
            unchecked { return (int)(uint)_data; }
        }
        public static implicit operator RenderKey(uint value) { return new RenderKey(value); }
        public static implicit operator uint(RenderKey value) { return value._data; }
    }
}
