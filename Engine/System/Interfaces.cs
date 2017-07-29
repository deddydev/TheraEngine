﻿using TheraEngine.Files;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using System;
using TheraEngine.Rendering;
using System.ComponentModel;

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
        bool VisibleByDefault { get; }
        Shape CullingVolume { get; }
        PrimitiveData Data { get; }
        Material Material { get; set; }
        RenderInfo3D RenderInfo { get; }
    }
    public interface ISkeletalSubMesh
    {
        bool VisibleByDefault { get; }
        SingleFileRef<PrimitiveData> Data { get; }
        Material Material { get; set; }
        RenderInfo3D RenderInfo { get; }
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class RenderInfo2D
    {
        /// <summary>
        /// Used by the engine for proper order of rendering.
        /// </summary> 
        public RenderPassType2D RenderPass;
        /// <summary>
        /// Used to render objects in the same pass in a certain order.
        /// Smaller value means rendered sooner, zero (exactly) means it doesn't matter.
        /// </summary>
        public int LayerIndex;
        public int OrderInLayer;
        
        public RenderInfo2D(RenderPassType2D pass, int layerIndex, int orderInLayer)
        {
            RenderPass = pass;
            LayerIndex = layerIndex;
            OrderInLayer = orderInLayer;
        }
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class RenderInfo3D
    {
        /// <summary>
        /// Used to render objects in the same pass in a certain order.
        /// Smaller value means rendered sooner, zero (exactly) means it doesn't matter.
        /// </summary>
        public float RenderOrder => RenderOrderFunc == null ? 0.0f : RenderOrderFunc();
        public bool ReceivesShadows { get; set; } = true;
        public bool CastsShadows { get; set; } = true;
        public ERenderPassType3D RenderPass { get; set; } = ERenderPassType3D.OpaqueDeferredLit;

        public Func<float> RenderOrderFunc;

        public RenderInfo3D(ERenderPassType3D pass, Func<float> renderOrderFunc, bool castsShadows = true, bool receivesShadows = true)
        {
            RenderPass = pass;
            RenderOrderFunc = renderOrderFunc;
            CastsShadows = castsShadows;
            ReceivesShadows = receivesShadows;
        }
    }
    /// <summary>
    /// Use this interface for objects you want to be able to render in 3D world space with the engine.
    /// </summary>
    public interface I3DRenderable : I3DBoundable
    {
        /// <summary>
        /// Called when the engine wants to render this object.
        /// </summary>
        void Render();
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
    public enum RenderPassType2D
    {
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
    public interface I2DRenderable : I2DBoundable
    {
        /// <summary>
        /// Called when the engine wants to render this object.
        /// </summary>
        void Render();
        /// <summary>
        /// Used to determine when to render this object.
        /// </summary>
        RenderInfo2D RenderInfo { get; }
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
