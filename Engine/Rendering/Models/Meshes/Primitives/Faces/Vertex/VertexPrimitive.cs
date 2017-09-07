using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.Models
{
    public abstract class VertexPrimitive : ObjectBase, IEnumerable<Vertex>
    {
        public abstract FaceType Type { get; }
        public ReadOnlyCollection<Vertex> Vertices { get { return _vertices.AsReadOnly(); } }

        protected List<Vertex> _vertices = new List<Vertex>();
        
        public VertexPrimitive(IEnumerable<Vertex> vertices) { _vertices = vertices.ToList(); }
        public VertexPrimitive(params Vertex[] vertices) { _vertices = vertices.ToList(); }

        public BoundingBox GetCullingVolume()
        {
            Vec3[] positions = _vertices.Select(x => x._position).ToArray();
            return BoundingBox.FromMinMax(CustomMath.ComponentMin(positions), CustomMath.ComponentMax(positions));
        }

        public IEnumerator<Vertex> GetEnumerator() { return ((IEnumerable<Vertex>)_vertices).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<Vertex>)_vertices).GetEnumerator(); }
    }
    public abstract class VertexPolygon : VertexPrimitive
    {
        public VertexPolygon(params Vertex[] vertices) : base(vertices)
        {
            if (vertices.Length < 3)
                throw new InvalidOperationException("Not enough vertices for a polygon.");
        }
        public VertexPolygon(IEnumerable<Vertex> vertices) : base(vertices)
        {
            if (Vertices.Count < 3)
                throw new InvalidOperationException("Not enough vertices for a polygon.");
        }
        public abstract VertexTriangle[] ToTriangles();
        public virtual VertexLine[] ToLines()
        {
            return ToTriangles().SelectMany(x => x.ToLines()).ToArray();
        }
    }
}
