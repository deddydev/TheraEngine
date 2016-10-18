using System;
using System.Collections.Generic;
using BulletSharp;

namespace CustomEngine.Rendering.Models
{
    public class Mesh : ObjectBase, IRenderable
    {
        private int _vao, _bufferCount;
        public CollisionShape _collision;
        public List<Polygon> _faceIndices;
        public List<Vertex> _facePoints;

        public void Render()
        {
            
        }

        public static Mesh FromShape(IShape shape)
        {

        }
    }
}
