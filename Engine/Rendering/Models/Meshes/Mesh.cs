using System;
using System.Collections.Generic;
using BulletSharp;

namespace CustomEngine.Rendering.Models
{
    public class Mesh : ObjectBase, IRenderable
    {
        private int _vao, _bufferCount;
        public CollisionShape _collision;
        public PrimitiveManager _manager;

        public void Render()
        {
            
        }
    }
}
