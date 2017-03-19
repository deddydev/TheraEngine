using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using System;

namespace CustomEngine
{
    public interface IStaticMesh
    {
        bool VisibleByDefault { get; }
        Shape CullingVolume { get; }
        PrimitiveData Data { get; }
        Material Material { get; set; }
    }
    public interface ISkeletalMesh
    {
        bool VisibleByDefault { get; }
        string SingleBindName { get; }
        PrimitiveData Data { get; }
        Material Material { get; set; }
    }
    public interface IRenderable : I3DBoundable
    {
        void Render();
    }
    public interface I3DBoundable
    {
        Shape CullingVolume { get; }
        Octree.Node RenderNode { get; set; }
        bool IsRendering { get; set; }
    }
    public interface I2DBoundable
    {
        BoundingRectangle AxisAlignedBounds { get; }
        Quadtree.Node RenderNode { get; set; }
        bool IsRendering { get; set; }
    }
    public interface IPanel
    {
        BoundingRectangle Region { get; set; }
        BoundingRectangle Resize(BoundingRectangle parentRegion);
    }
    public interface IBufferable
    {
        VertexBuffer.ComponentType ComponentType { get; }
        int ComponentCount { get; }
        bool Normalize { get; }
        void Write(VoidPtr address);
        void Read(VoidPtr address);
    }
    
    public interface IGLVarOwner { }
    public interface IUniformable { }
    
    public unsafe interface IUniformable1Bool : IUniformable { bool* Data { get; } }
    public unsafe interface IUniformable1Int : IUniformable { int* Data { get; } }
    public unsafe interface IUniformable1UInt : IUniformable { uint* Data { get; } }
    public unsafe interface IUniformable1Float : IUniformable { float* Data { get; } }
    public unsafe interface IUniformable1Double : IUniformable { double* Data { get; } }

    public unsafe interface IUniformable2Bool : IUniformable { bool* Data { get; } }
    public unsafe interface IUniformable2Int : IUniformable { int* Data { get; } }
    public unsafe interface IUniformable2UInt : IUniformable { uint* Data { get; } }
    public unsafe interface IUniformable2Float : IUniformable { float* Data { get; } }
    public unsafe interface IUniformable2Double : IUniformable { double* Data { get; } }

    public unsafe interface IUniformable3Bool : IUniformable { bool* Data { get; } }
    public unsafe interface IUniformable3Int : IUniformable { int* Data { get; } }
    public unsafe interface IUniformable3UInt : IUniformable { uint* Data { get; } }
    public unsafe interface IUniformable3Float : IUniformable { float* Data { get; } }
    public unsafe interface IUniformable3Double : IUniformable { double* Data { get; } }

    public unsafe interface IUniformable4Bool : IUniformable { bool* Data { get; } }
    public unsafe interface IUniformable4Int : IUniformable { int* Data { get; } }
    public unsafe interface IUniformable4UInt : IUniformable { uint* Data { get; } }
    public unsafe interface IUniformable4Float : IUniformable { float* Data { get; } }
    public unsafe interface IUniformable4Double : IUniformable { double* Data { get; } }

    public unsafe interface IUniformableArray : IUniformable
    {
        IUniformable[] Values { get; }
    }
}
