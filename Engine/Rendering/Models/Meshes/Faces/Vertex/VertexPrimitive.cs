﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CustomEngine.Rendering.Models
{
    public abstract class VertexPrimitive : ObjectBase, IEnumerable<Vertex>
    {
        public abstract FaceType Type { get; }
        public ReadOnlyCollection<Vertex> Vertices { get { return _vertices.AsReadOnly(); } }

        protected List<Vertex> _vertices = new List<Vertex>();

        public VertexPrimitive(params Vertex[] vertices) { _vertices = vertices.ToList(); }

        public Box GetCullingVolume()
        {
            Vec3[] positions = _vertices.Select(x => x.BaseVertex._position).ToArray();
            return new Box(CustomMath.ComponentMin(positions), CustomMath.ComponentMax(positions));
        }

        public IEnumerator<Vertex> GetEnumerator() { return ((IEnumerable<Vertex>)_vertices).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<Vertex>)_vertices).GetEnumerator(); }
    }
    public abstract class VertexPolygon : VertexPrimitive
    {
        public VertexPolygon(params Vertex[] vertices) : base(vertices) { }
        public abstract List<VertexTriangle> ToTriangles();
    }
}
