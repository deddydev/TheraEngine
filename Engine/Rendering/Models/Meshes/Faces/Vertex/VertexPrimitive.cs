using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CustomEngine.Rendering.Models
{
    public abstract class VertexPrimitive : ObjectBase, IEnumerable<RawVertex>
    {
        public abstract FaceType Type { get; }
        public ReadOnlyCollection<RawVertex> Vertices { get { return _vertices.AsReadOnly(); } }

        protected List<RawVertex> _vertices = new List<RawVertex>();
        public VertexPrimitive(params RawVertex[] vertices)
        {
            _vertices = vertices.ToList();
        }

        public IEnumerator<RawVertex> GetEnumerator()
        {
            return ((IEnumerable<RawVertex>)_vertices).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<RawVertex>)_vertices).GetEnumerator();
        }
    }
    public abstract class VertexPolygon : VertexPrimitive
    {
        public VertexPolygon(params RawVertex[] vertices) : base(vertices) { }
        public abstract List<VertexTriangle> ToTriangles();
    }
}
