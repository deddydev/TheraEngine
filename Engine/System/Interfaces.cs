using TheraEngine.Files;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using System;

namespace TheraEngine
{
    public interface IParsable
    {
        string WriteToString();
        void ReadFromString(string str);
    }
    public interface IModelFile
    {

    }
    public interface ISubMesh : I3DRenderable
    {
        bool Visible { get; set; }
        bool VisibleInEditorOnly { get; set; }
        bool HiddenFromOwner { get; set; }
        bool VisibleToOwnerOnly { get; set; }
    }
    public interface IStaticSubMesh
    {
        bool Visible { get; }
        Shape CullingVolume { get; }
        PrimitiveData Data { get; }
        Material Material { get; set; }
        StaticMesh Model { get; }
    }
    public interface ISkeletalSubMesh
    {
        bool Visible { get; }
        SingleFileRef<PrimitiveData> Data { get; }
        Material Material { get; set; }
        SkeletalMesh Model { get; }
    }
    /// <summary>
    /// Use this interface for objects you want to be able to render in 3D world space with the engine.
    /// </summary>
    public interface I3DRenderable : I3DBoundable
    {
        /// <summary>
        /// /// Called when the engine wants to render this object.
        /// /// </summary>
        void Render();
        /// <summary>
        /// Used by the engine for proper order of rendering opaque and transparent objects.
        /// </summary>
        bool HasTransparency { get; }
    }
    /// <summary>
    /// Used by octrees to set the visibility of this object on camera. Does not need to be renderable necessarily (use I3DRenderable for that).
    /// </summary>
    public interface I3DBoundable
    {
        /// <summary>
        /// The shape the rendering octree will use to determine occlusion and offscreen culling (visibility).
        /// </summary>
        Shape CullingVolume { get; }
        /// <summary>
        /// The octree bounding box this object is currently located in.
        /// </summary>
        IOctreeNode OctreeNode { get; set; }
        /// <summary>
        /// True if the object is not occluded and within the camera view.
        /// </summary>
        bool IsRendering { get; set; }
    }
    /// <summary>
    /// Use this interface for objects you want to be able to render in 2D HUD space with the engine.
    /// </summary>
    public interface I2DRenderable : I2DBoundable
    {
        /// <summary>
        /// Called when the engine wants to render this object.
        /// </summary>
        void Render();
        /// <summary>
        /// Used by the engine for proper order of rendering opaque and transparent objects.
        /// </summary>
        bool HasTransparency { get; }
        /// <summary>
        /// The depth of this object on screen, where 0 is beneath everything else.
        /// </summary>
        ushort LayerIndex { get; }
    }
    public interface I2DBoundable
    {
        BoundingRectangle AxisAlignedBounds { get; }
        IQuadtreeNode QuadtreeNode { get; set; }
        bool IsRendering { get; set; }
        bool Contains(Vec2 point);
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
    
    public interface IShaderVarOwner { }
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

    public unsafe interface IUniformableArray : IUniformable { IUniformable[] Values { get; } }
}
