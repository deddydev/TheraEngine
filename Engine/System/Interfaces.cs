using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using System;
using System.Drawing;
using System.Threading.Tasks;
using CustomEngine.Rendering.Animation;

namespace CustomEngine
{
    public interface IRenderable
    {
        void Render(float delta);
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
    public interface IShape : IRenderable
    {
        bool ContainsPoint(Vec3 point);
        PrimitiveData GetPrimitives();
    }

    public interface IBufferable
    {
        VertexBuffer.ComponentType ComponentType { get; }
        int ComponentCount { get; }
        bool Normalize { get; }
        void Write(VoidPtr address);
    }

    public interface IGLVarOwner { }

    public unsafe interface IUniformable { }

    public unsafe interface IUniformable1Int : IUniformable { int* Data { get; } }
    //public unsafe interface IUniformable1UInt { uint* Address { get; } }
    public unsafe interface IUniformable1Float : IUniformable { float* Data { get; } }
    //public unsafe interface IUniformable1Double { double* Address { get; } }

    public unsafe interface IUniformable2Int : IUniformable { int* Data { get; } }
    //public unsafe interface IUniformable2UInt { uint* Address { get; } }
    public unsafe interface IUniformable2Float : IUniformable { float* Data { get; } }
    //public unsafe interface IUniformable2Double { double* Address { get; } }

    public unsafe interface IUniformable3Int : IUniformable { int* Data { get; } }
    //public unsafe interface IUniformable3UInt { uint* Address { get; } }
    public unsafe interface IUniformable3Float : IUniformable { float* Data { get; } }
    //public unsafe interface IUniformable3Double { double* Address { get; } }
    
    public unsafe interface IUniformable4Int : IUniformable { int* Data { get; } }
    //public unsafe interface IUniformable4UInt { uint* Address { get; } }
    public unsafe interface IUniformable4Float : IUniformable { float* Data { get; } }
    //public unsafe interface IUniformable4Double { double* Address { get; } }
}
