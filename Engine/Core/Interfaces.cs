using TheraEngine.Files;
using TheraEngine.Rendering.Models;
using System;
using TheraEngine.Rendering;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using System.Collections.Generic;
using TheraEngine.Core.Memory;

namespace TheraEngine
{
    public interface ITextSource
    {
        string Text { get; set; }
    }
    public interface IParsable
    {
        string WriteToString();
        void ReadFromString(string str);
    }
    public interface IModelFile
    {

    }
    public interface IBaseSubMesh : IObjectBase
    {
        bool VisibleByDefault { get; set; }
        List<LOD> LODs { get; }
        RenderInfo3D RenderInfo { get; set; }
    }
    public interface IStaticSubMesh : IBaseSubMesh
    {
        GlobalFileRef<Shape> CullingVolumeRef { get; }
    }
    public interface ISkeletalSubMesh : IBaseSubMesh
    {

    }
    /// <summary>
    /// Use this interface for objects you want to be able to render in 3D world space with the engine.
    /// </summary>
    public interface I3DRenderable : I3DBoundable, IRenderable
    {
        /// <summary>
        /// Used to determine when to render this object.
        /// </summary>
        RenderInfo3D RenderInfo { get; }
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
    }
    public enum ERenderPass2D
    {
        /// <summary>
        /// Use for background objects that don't write to depth.
        /// </summary>
        Background,
        /// <summary>
        /// Use for any fully opaque objects.
        /// </summary>
        Opaque,
        /// <summary>
        /// Use for all objects that use alpha translucency! Material.HasTransparency will help you determine this.
        /// </summary>
        Transparent,
        /// <summary>
        /// Renders on top of everything that has been previously rendered.
        /// </summary>
        OnTop,
    }
    /// <summary>
    /// Use this interface for objects you want to be able to render in 2D HUD space with the engine.
    /// </summary>
    public interface I2DRenderable : I2DBoundable, IRenderable
    {
        /// <summary>
        /// Used to determine when to render this object.
        /// </summary>
        RenderInfo2D RenderInfo { get; }
    }
    public interface I2DBoundable
    {
        /// <summary>
        /// The axis-aligned bounding box for this UI component.
        /// </summary>
        BoundingRectangle AxisAlignedRegion { get; }
        IQuadtreeNode QuadtreeNode { get; set; }
        //bool Contains(Vec2 point);
    }
    public interface IRenderable
    {
        /// <summary>
        /// Called when the engine wants to render this object.
        /// </summary>
        void Render();
    }
    public interface IBufferable
    {
        VertexBuffer.ComponentType ComponentType { get; }
        int ComponentCount { get; }
        bool Normalize { get; }
        void Write(VoidPtr address);
        void Read(VoidPtr address);
    }

    public interface IUniformable { }
    public interface IShaderVarOwner : IUniformable { }
    public interface IUniformableArray : IUniformable { }

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

    public unsafe interface IUniformableArray<T> : IUniformableArray where T : IUniformable { T[] Values { get; } }
}
