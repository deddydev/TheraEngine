using System;
using System.Collections.Generic;
using BulletSharp;

namespace CustomEngine.Rendering.Models
{
    public class Mesh : ObjectBase, IRenderable
    {
        public int _vao, _bufferCount;
        public CollisionShape _collision;

        public void Render()
        {
            
        }
    }
}
