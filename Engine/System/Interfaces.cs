using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace CustomEngine
{
    public interface IRenderable
    {
        void Render();
    }
    public interface ITransformable
    {
        FrameState Transform { get; set; }
    }
    public interface IPanel
    {
        RectangleF Region { get; set; }
        void OnResized();
    }
    public interface IPropertyAnimation
    {
        IKeyframeTrack Keyframes { get; }
        int FrameCount { get; set; }
    }
    public interface IKeyframeTrack
    {

    }
    public interface IRenderState
    {

    }
    public interface IShape : IRenderable
    {
        bool ContainsPoint(Vec3 point);
    }

    public interface IGLVarOwner { }

    public unsafe interface IUniformable { }

    public unsafe interface IUniformable1Int : IUniformable { int* Address { get; } }
    //public unsafe interface IUniformable1UInt { uint* Address { get; } }
    public unsafe interface IUniformable1Float : IUniformable { float* Address { get; } }
    //public unsafe interface IUniformable1Double { double* Address { get; } }

    public unsafe interface IUniformable2Int : IUniformable { int* Address { get; } }
    //public unsafe interface IUniformable2UInt { uint* Address { get; } }
    public unsafe interface IUniformable2Float : IUniformable { float* Address { get; } }
    //public unsafe interface IUniformable2Double { double* Address { get; } }

    public unsafe interface IUniformable3Int : IUniformable { int* Address { get; } }
    //public unsafe interface IUniformable3UInt { uint* Address { get; } }
    public unsafe interface IUniformable3Float : IUniformable { float* Address { get; } }
    //public unsafe interface IUniformable3Double { double* Address { get; } }
    
    public unsafe interface IUniformable4Int : IUniformable { int* Address { get; } }
    //public unsafe interface IUniformable4UInt { uint* Address { get; } }
    public unsafe interface IUniformable4Float : IUniformable { float* Address { get; } }
    //public unsafe interface IUniformable4Double { double* Address { get; } }
}
