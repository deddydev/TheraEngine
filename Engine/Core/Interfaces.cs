using System;
using System.Collections.Generic;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Memory;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;

namespace TheraEngine
{
    public interface ITextSource
    {
        string Text { get; set; }
    }
    /// <summary>
    /// This object can be serialized as a string.
    /// </summary>
    public interface ISerializableString
    {
        string WriteToString();
        void ReadFromString(string str);
    }
    /// <summary>
    /// This object can be serialized as a byte array.
    /// </summary>
    public interface ISerializableByteArray
    {
        byte[] WriteToBytes();
        void ReadFromBytes(byte[] bytes);
    }
    /// <summary>
    /// This object can be serialized to/from a pointer.
    /// </summary>
    public interface ISerializablePointer
    {
        int GetSize();
        void WriteToPointer(VoidPtr address);
        void ReadFromPointer(VoidPtr address, int size);
    }
    public interface IModelFile
    {

    }
    public interface IBaseSubMesh : IObject
    {
        IEventList<ILOD> LODs { get; }
        ERenderPass RenderPass { get; set; }
        IRenderInfo3D RenderInfo { get; set; }
    }
    public interface IStaticSubMesh : IBaseSubMesh
    {
    }
    public interface ISkeletalSubMesh : IBaseSubMesh
    {
    }
    /// <summary>
    /// Use this interface for objects you want to be able to render in 3D world space with the engine.
    /// </summary>
    public interface I3DRenderable : IRenderable
    {
        /// <summary>
        /// Used to determine when and how to render this object.
        /// </summary>
        IRenderInfo3D RenderInfo { get; }
        void AddRenderables(RenderPasses passes, ICamera camera);
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
    public interface I2DRenderable : IRenderable
    {
        /// <summary>
        /// Used to determine when to render this object.
        /// </summary>
        IRenderInfo2D RenderInfo { get; }
        void AddRenderables(RenderPasses passes, ICamera camera);

        bool Contains(Vec2 worldPoint);
        Vec2 ClosestPoint(Vec2 worldPoint);
    }
    public interface IRenderable
    {
        /// <summary>
        /// Called when the engine wants to render this object.
        /// </summary>
        //void Render();
    }
    public interface IBufferable
    {
        DataBuffer.EComponentType ComponentType { get; }
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
