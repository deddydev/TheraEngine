using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CustomEngine.Rendering.Models
{
    public abstract class VertexPolygon : ObjectBase
    {
        public abstract FaceType Type { get; }
        public ReadOnlyCollection<Vertex> Vertices { get { return _vertices.AsReadOnly(); } }

        protected List<Vertex> _vertices = new List<Vertex>();
        public VertexPolygon(params Vertex[] vertices)
        {
            _vertices = vertices.ToList();
        }
        
        public abstract List<VertexTriangle> ToTriangles();
    }
}
