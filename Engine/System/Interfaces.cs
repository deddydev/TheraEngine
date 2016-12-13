using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using System;
using System.Drawing;
using System.Threading.Tasks;
using CustomEngine.Rendering.Animation;
using System.Collections.Generic;
using CustomEngine.Rendering;
using CustomEngine.Worlds.Actors.Components;
using System.Collections.ObjectModel;
using BulletSharp;

namespace CustomEngine
{
    public interface ICollidable
    {
        RigidBody CollisionObject { get; set; }
    }
    public interface IPanel
    {
        RectangleF Region { get; set; }
        RectangleF ParentResized(RectangleF parentRegion);
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
