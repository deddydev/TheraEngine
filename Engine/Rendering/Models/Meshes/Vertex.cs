using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models
{
    public class Vertex : ObjectBase
    {
        public Vec3 _position;
        public Vec3? _normal;
        public List<Vec2> _texCoords;
        public List<RGBAPixel> _colors;
    }

    public enum OutsideDirection
    {
        Toward,
        AwayFrom
    }

    public class VertexTriangle
    {
        public Vertex _v0, _v1, _v2;
        public OutsideDirection _outside;

        public VertexTriangle(Vertex v0, Vertex v1, Vertex v2, OutsideDirection outside)
        {
            _v0 = v0;
            _v1 = v1;
            _v2 = v2;
            _outside = outside;
        }

        public List<Vertex> GetVertexList()
        {
            if (_outside == OutsideDirection.Toward)
                return new List<Vertex>() { _v0, _v1, _v2 };
            else
                return new List<Vertex>() { _v0, _v2, _v1 };
        }
    }

    public class VertexQuad
    {
        public Vertex _v0, _v1, _v2, _v3;
        public OutsideDirection _outside;
        public bool _forwardSlash = true;

        public VertexQuad(Vertex v0, Vertex v1, Vertex v2, Vertex v3, OutsideDirection outside)
        {
            _v0 = v0;
            _v1 = v1;
            _v2 = v2;
            _v3 = v3;
            _outside = outside;
        }

        public List<VertexTriangle> GetTriangleList()
        {
            if (_outside == OutsideDirection.Toward)
                return new List<VertexTriangle>()
                {
                    new VertexTriangle(_v0, _v1, _v2, OutsideDirection.Toward),
                    new VertexTriangle(_v1, _v3, _v2, OutsideDirection.Toward)
                };
            else
                return new List<VertexTriangle>()
                {
                    new VertexTriangle(_v0, _v2, _v1, OutsideDirection.AwayFrom),
                    new VertexTriangle(_v1, _v2, _v3, OutsideDirection.AwayFrom)
                };
        }
    }
}
