using System;
using System.Collections.Generic;
using BulletSharp;

namespace CustomEngine.Rendering.Models
{
    public class Mesh : ObjectBase, IRenderable
    {
        public CollisionShape _collision;
        public PrimitiveManager _manager;
        public Materials.Material _material;

        public void Render() { _manager.Render(); }
    }
}
