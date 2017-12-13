using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.Models
{
    public abstract class VertexPrimitive : TObject, IEnumerable<Vertex>
    {
        public abstract FaceType Type { get; }
        public ReadOnlyCollection<Vertex> Vertices { get { return _vertices.AsReadOnly(); } }

        protected List<Vertex> _vertices = new List<Vertex>();
        
        public VertexPrimitive(IEnumerable<Vertex> vertices) { _vertices = vertices.ToList(); }
        public VertexPrimitive(params Vertex[] vertices) { _vertices = vertices.ToList(); }

        public BoundingBox GetCullingVolume()
        {
            Vec3[] positions = _vertices.Select(x => x._position).ToArray();
            return BoundingBox.FromMinMax(Vec3.ComponentMin(positions), Vec3.ComponentMax(positions));
        }

        public IEnumerator<Vertex> GetEnumerator() => ((IEnumerable<Vertex>)_vertices).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Vertex>)_vertices).GetEnumerator();
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
