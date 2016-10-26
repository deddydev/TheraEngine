using System;
using System.Collections.Generic;
using BulletSharp;

namespace CustomEngine.Rendering.Models
{
    public class Mesh : ObjectBase, IRenderable
    {
        public Mesh(Model owner) { _owner = owner; }

        public Model _owner;
        public CollisionShape _collision;
        public PrimitiveManager _manager;
        public Materials.Material _material;

        public void Render(float delta) { _manager.Render(); }
    }
}
