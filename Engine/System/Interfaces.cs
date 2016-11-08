using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using System;
using System.Drawing;
using System.Threading.Tasks;
using CustomEngine.Rendering.Animation;
using System.Collections.Generic;
using CustomEngine.Rendering;

namespace CustomEngine
{
    public interface IRenderable
    {
        void Render();
    }
    public interface ITransformable
    {
        FrameState LocalTransform { get; set; }
    }
    public interface IPanel
    {
        RectangleF Region { get; set; }
        RectangleF ParentResized(RectangleF parentRegion);
    }
    public interface IPrimitive
    {
        List<PrimitiveData> GetPrimitives();
    }
    public interface IShape : IPrimitive
    {
        bool ContainsPoint(Vec3 point);
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
    
    public unsafe interface IUniformable1Bool : IUniformable { bool* Data { get; } }
    public unsafe interface IUniformable1Int : IUniformable { int* Data { get; } }
    public unsafe interface IUniformable1UInt { uint* Data { get; } }
    public unsafe interface IUniformable1Float : IUniformable { float* Data { get; } }
    public unsafe interface IUniformable1Double { double* Data { get; } }

    public unsafe interface IUniformable2Bool : IUniformable { bool* Data { get; } }
    public unsafe interface IUniformable2Int : IUniformable { int* Data { get; } }
    public unsafe interface IUniformable2UInt { uint* Data { get; } }
    public unsafe interface IUniformable2Float : IUniformable { float* Data { get; } }
    public unsafe interface IUniformable2Double { double* Data { get; } }

    public unsafe interface IUniformable3Bool : IUniformable { bool* Data { get; } }
    public unsafe interface IUniformable3Int : IUniformable { int* Data { get; } }
    public unsafe interface IUniformable3UInt { uint* Data { get; } }
    public unsafe interface IUniformable3Float : IUniformable { float* Data { get; } }
    public unsafe interface IUniformable3Double { double* Data { get; } }

    public unsafe interface IUniformable4Bool : IUniformable { bool* Data { get; } }
    public unsafe interface IUniformable4Int : IUniformable { int* Data { get; } }
    public unsafe interface IUniformable4UInt { uint* Data { get; } }
    public unsafe interface IUniformable4Float : IUniformable { float* Data { get; } }
    public unsafe interface IUniformable4Double { double* Data { get; } }
}
