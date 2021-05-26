using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.Models
{
    public abstract class TVertexPrimitive : TObject, IEnumerable<TVertex>
    {
        public abstract FaceType Type { get; }
        public ReadOnlyCollection<TVertex> Vertices => _vertices.AsReadOnly();

        protected List<TVertex> _vertices = new List<TVertex>();
        
        public TVertexPrimitive(IEnumerable<TVertex> vertices) 
            => _vertices = vertices.ToList();
        public TVertexPrimitive(params TVertex[] vertices)
            => _vertices = vertices.ToList();

        public BoundingBox GetCullingVolume()
        {
            Vec3[] positions = _vertices.Select(x => x.Position).ToArray();
            return BoundingBox.FromMinMax(Vec3.ComponentMin(positions), Vec3.ComponentMax(positions));
        }

        public IEnumerator<TVertex> GetEnumerator() => ((IEnumerable<TVertex>)_vertices).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TVertex>)_vertices).GetEnumerator();
    }
    public class TVertexPolygon : TVertexPrimitive
    {
        public override FaceType Type => FaceType.Ngon;
        public TVertexPolygon(params TVertex[] vertices) : base(vertices)
        {
            if (vertices.Length < 3)
                throw new InvalidOperationException("Not enough vertices for a polygon.");
        }
        public TVertexPolygon(IEnumerable<TVertex> vertices) : base(vertices)
        {
            if (Vertices.Count < 3)
                throw new InvalidOperationException("Not enough vertices for a polygon.");
        }
        /// <summary>
        /// Example polygons:
        ///   4----3
        ///  /      \
        /// 5        2
        ///  \      /
        ///   0----1
        /// Converted: 012, 023, 034, 045
        /// 3---2
        /// |   |
        /// 0---1
        /// Converted: 012, 023
        /// </summary>
        public virtual TVertexTriangle[] ToTriangles()
        {
            int triangleCount = Vertices.Count - 2;
            if (triangleCount < 1)
                return new TVertexTriangle[0];
            TVertexTriangle[] list = new TVertexTriangle[triangleCount];
            for (int i = 0; i < triangleCount; ++i)
                list[i] = new TVertexTriangle(Vertices[0].HardCopy(), Vertices[i + 1].HardCopy(), Vertices[i + 2].HardCopy());
            return list;
        }
        public virtual TVertexLine[] ToLines()
        {
            TVertexLine[] lines = new TVertexLine[Vertices.Count];
            for (int i = 0; i < Vertices.Count - 1; ++i)
                lines[i] = new TVertexLine(Vertices[i].HardCopy(), Vertices[i + 1].HardCopy());
            lines[Vertices.Count - 1] = new TVertexLine(Vertices[Vertices.Count - 1].HardCopy(), Vertices[0].HardCopy());
            return lines;
            //return ToTriangles().SelectMany(x => x.ToLines()).ToArray();
        }
    }
}
